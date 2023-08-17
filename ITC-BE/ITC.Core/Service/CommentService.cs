using System;
using AutoMapper;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using ITC.Core.Data;
using ITC.Core.Interface;
using ITC.Core.Model;
using ITC.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ITC.Core.Service
{
    public class CommentService : ICommentService
    {
        private readonly ITCDBContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;
        private readonly IFirebaseFCMService _serFireBase;
        private readonly string _storageConnectionString;
        private readonly string _storageContainerName;
        private readonly ILogger<AzureBlobStorageService> _logger;
        public CommentService(ITCDBContext context, IMapper mapper
                                , IUserContextService userContextService
                                , IFirebaseFCMService serFireBase
                                , IConfiguration configuration, ILogger<AzureBlobStorageService> logger)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
            _serFireBase = serFireBase;
            _logger = logger;
            _storageConnectionString = configuration["BlobConnectionString"];
            _storageContainerName = configuration["BlobContainerName"];

        }

        public async Task<ResultModel> CreateTaskComment(CommentUploadApiRequest model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            CommentResponse response = new();
            BlobContainerClient container = new BlobContainerClient(_storageConnectionString, _storageContainerName);
            var FileId = Guid.NewGuid();
            try
            {
                var staff = _context.Staff.FirstOrDefault(x => x.AccountId == _userContextService.UserID);
                if (model.FormFile == null)
                {
                    var comment = new CommentTask
                    {
                        Id = FileId,
                        StaffId = staff?.Id,
                        FullName = _userContextService.FullName,
                        Comment = model.Comment,
                        DateCreated = DateTime.Now,
                        FileUrl = null,
                        TasksId = model.TaskId,
                        CheckEdit = false
                    };
                    await _context.CommentTask.AddAsync(comment);
                    await _context.SaveChangesAsync();
                    var tasks = await _context.Tasks.Where(x => x.Id == model.TaskId).FirstOrDefaultAsync();
                    var checkStaff = await _context.AssignTask.Where(x => x.TasksId == model.TaskId).ToListAsync();
                    if (checkStaff != null)
                    {
                        foreach (var item in checkStaff)
                        {
                            var checkAccount = await _context.Staff.Where(x => x.Id == item.StaffId).FirstOrDefaultAsync();
                            var noti = new Notification
                            {
                                Id = Guid.NewGuid(),
                                Title = "Comment",
                                UserSendId = _userContextService.UserID,
                                FullName = comment.FullName,
                                DateCreate = DateTime.Now,
                                TaskCommnetId = comment.Id,
                                AccountId = checkAccount!.AccountId,
                                TasksId = comment.TasksId,
                                Read = false,
                                Body = comment.FullName + "Comment In Task:" + item.Tasks.TaskName,
                            };

                            await _context.Notification.AddAsync(noti);
                            await _context.SaveChangesAsync();
                            await _serFireBase.PushNotification(noti.Id, noti.UserSendId, noti.AccountId, comment.Id, comment.TasksId, null, noti.Body);
                        }
                    }
                }

                if (model.FormFile != null)
                {
                    await container.CreateIfNotExistsAsync();
                    var fileextension = Path.GetExtension(model.FormFile?.FileName);
                    var filename = model.FormFile?.FileName;
                    BlobClient client = container.GetBlobClient(filename);
                    var co = from n in _context.FileTracking
                             where n.FileName + n.FileExtension == model.FormFile.FileName
                             select n;
                    int num = co.Count();
                    var newFileName = model.FormFile?.FileName.Split(".").First() + " " + "(" + (num + 1) + ")" + "." + model.FormFile?.FileName.Split(".").Last();

                    if (client.Exists())
                    {
                        client = container.GetBlobClient(newFileName);
                    }
                    await using (Stream? data = model.FormFile?.OpenReadStream())
                    {
                        await client.UploadAsync(data);
                    }
                    response.Status = $"File {model.FormFile?.FileName} Uploaded Successfully";
                    response.Error = false;
                    response.Uri = client.Uri.AbsoluteUri;
                    response.Name = client.Name;
                    var FileTracking = new FileTracking
                    {
                        TraceId = FileId,
                        FileName = response.Name,
                        FileExtension = model.FormFile?.FileName.Split(".").Last(),
                        FileUrl = response.Uri,
                        Owner = _userContextService.FullName,
                        DateUpLoad = DateTime.Now
                    };
                    var commentWithFile = new CommentTask
                    {
                        Id = FileId,
                        StaffId = staff?.Id,
                        FullName = _userContextService.FullName,
                        Comment = model.Comment,
                        DateCreated = DateTime.Now,
                        FileUrl = response.Uri,
                        TasksId = model.TaskId,
                        CheckEdit = false
                    };
                    await _context.CommentTask.AddAsync(commentWithFile);
                    await _context.FileTracking.AddAsync(FileTracking);
                    await _context.SaveChangesAsync();

                    var tasks = await _context.Tasks.Where(x => x.Id == model.TaskId).FirstOrDefaultAsync();
                    var checkStaff = await _context.AssignTask.Where(x => x.TasksId == model.TaskId).ToListAsync();
                    if (checkStaff != null)
                    {
                        foreach (var item in checkStaff)
                        {
                            var checkAccount = await _context.Staff.Where(x => x.Id == item.StaffId).FirstOrDefaultAsync();
                            var noti = new Notification
                            {
                                Id = Guid.NewGuid(),
                                UserSendId = _userContextService.UserID,
                                FullName = commentWithFile.FullName,
                                DateCreate = DateTime.Now,
                                TaskCommnetId = commentWithFile.Id,
                                AccountId = checkAccount!.AccountId,
                                TasksId = commentWithFile.TasksId,
                                Read = false,
                                Body = commentWithFile.FullName + "Comment In Task:" + item.Tasks.TaskName,
                            };

                            await _context.Notification.AddAsync(noti);
                            await _context.SaveChangesAsync();
                            await _serFireBase.PushNotification(noti.Id, noti.UserSendId, noti.AccountId, commentWithFile.Id, commentWithFile.TasksId, null, noti.Body);
                        }
                    }
                }


                //await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Code = 200;
                await transaction.CommitAsync();
            }
            catch (RequestFailedException ex)
               when (ex.ErrorCode == BlobErrorCode.BlobAlreadyExists)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"File with name {model.FormFile?.FileName} already exists in container. Set another name to store the file in the container: '{_storageContainerName}.'");
                response.Status = $"File with name {model.FormFile?.FileName} already exists. Please use another name to store your file.";
                response.Error = true;
                result.IsSuccess = false;
                result.ResponseFailed = "Document already exists. Please use another name to store your file";
            }
            catch (Exception e)
            {

                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;

            }
            return result;
        }

        public async Task<ResultModel> GetCommentByTaskId(Guid taskId)
        {
            var result = new ResultModel();
            try
            {

                var comment = await _context.CommentTask.Where(x => x.TasksId == taskId).ToListAsync();
                if (comment == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any comment Not Found!";
                    return result;
                }
                var respon = new List<CommentViewModel>();
                foreach (var item in comment)
                {
                    var checkFile = await _context.FileTracking.Where(x => x.TraceId == item.Id).ToListAsync();
                    var rs = new CommentViewModel()
                    {
                        Id = item.Id,
                        FullName = item.FullName,
                        Comment = item.Comment,
                        StaffId = item.StaffId,
                        TasksId = item.TasksId,
                        DateCreated = item.DateCreated,
                        FileUrl = item.FileUrl,
                        CheckEdit = item.CheckEdit,
                        ViewFile = checkFile.Select(x => new CommentViewModel.ViewFileTracking()
                        {
                            FileUrl = x.FileUrl,
                            FileName = x.FileName,
                            FileExtension = x.FileExtension,
                            Owner = x.Owner,
                            DateUpLoad = x.DateUpLoad

                        }).ToList()

                    };
                    respon.Add(rs);
                }
                if (respon.Count != 0)
                {
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.ResponseSuccess = respon.OrderByDescending(x=>x.DateCreated);
                }

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> GetTaskComment()
        {
            var result = new ResultModel();
            try
            {
                var comment = _context.CommentTask;

                if (comment == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Comment Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.CommentTask.Include(x => x.Tasks)
                                                                    .ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }


        public async Task<ResultModel> UpdateTaskComment(Guid Id, CommentUploadApiRequest model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            CommentResponse response = new();
            BlobContainerClient container = new BlobContainerClient(_storageConnectionString, _storageContainerName);
            try
            {
                var cmt = await _context.CommentTask.FirstOrDefaultAsync(x => x.Id == Id);
                var conf = await _context.FileTracking.FirstOrDefaultAsync(x => x.TraceId == Id);
                if (cmt == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = "can't find commnent";
                    return result;
                }
                if (model.FormFile == null)
                {
                    cmt.Comment = model.Comment;
                }
                if (model.FormFile != null)
                {
                    await container.CreateIfNotExistsAsync();
                    var fileextension = Path.GetExtension(model.FormFile?.FileName);
                    var filename = model.FormFile?.FileName;
                    BlobClient client = container.GetBlobClient(filename);
                    var co = from n in _context.FileTracking
                             where n.FileName + n.FileExtension == model.FormFile.FileName
                             select n;
                    int num = co.Count();
                    var newFileName = model.FormFile?.FileName.Split(".").First() + " " + "(" + (num + 1) + ")" + "." + model.FormFile?.FileName.Split(".").Last();
                    if (client.Exists())
                    {
                        client = container.GetBlobClient(newFileName);
                    }
                    await using (Stream? data = model.FormFile?.OpenReadStream())
                    {
                        await client.UploadAsync(data);
                    }
                    response.Status = $"File {model.FormFile?.FileName} Uploaded Successfully";
                    response.Error = false;
                    response.Uri = client.Uri.AbsoluteUri;
                    response.Name = client.Name;
                    var fileTracking = new FileTracking
                    {
                        TraceId = Id,
                        FileName = response.Name,
                        FileExtension = model.FormFile?.FileName.Split(".").Last(),
                        FileUrl = response.Uri,
                        Owner = _userContextService.FullName,
                        DateUpLoad = DateTime.Now
                    };
                    cmt.Comment = model.Comment;
                    cmt.FileUrl = response.Uri;
                    cmt.CheckEdit = true;
                    await _context.FileTracking.AddAsync(fileTracking);

                }
                _context.CommentTask.Update(cmt);
                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Code = 200;
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }


        public async Task<ResultModel> DeleteFileAsync(Guid Id)
        {

            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();

            BlobContainerClient client = new BlobContainerClient(_storageConnectionString, _storageContainerName);
            try
            {
                var conf = await _context.FileTracking.FirstOrDefaultAsync(x => x.TraceId == Id);
                var com = await _context.CommentTask.FirstOrDefaultAsync(x => x.Id == Id);
                var fileName = conf.FileName;
                BlobClient file = client.GetBlobClient(fileName);
                await file.DeleteAsync();
                if (com != null)
                {
                    com.FileUrl = null;
                    _context.CommentTask.Update(com);

                }
                _context.FileTracking.Remove(conf);
                await _context.SaveChangesAsync();

                result.Code = 200;
                result.ResponseSuccess = "Succesfull";
                result.IsSuccess = true;
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;

        }

        public async Task<ResultModel> DeleteTaskComment(Guid Id)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            BlobContainerClient client = new BlobContainerClient(_storageConnectionString, _storageContainerName);
            try
            {
                var cmt = await _context.CommentTask.FirstOrDefaultAsync(x => x.Id == Id);
                var conf = await _context.FileTracking.FirstOrDefaultAsync(x => x.TraceId == Id);
                if (cmt == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = "can't find commnent";
                    return result;
                }
                if (conf != null)
                {
                    var fileName = conf.FileName;
                    BlobClient file = client.GetBlobClient(fileName);
                    await file.DeleteAsync();
                    _context.FileTracking.Remove(conf);
                }
                _context.CommentTask.Remove(cmt);

                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Code = 200;
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }
    }
}
