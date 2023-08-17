using System;
using AutoMapper;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using ITC.Core.Data;
using ITC.Core.Interface;
using ITC.Core.Model;
using ITC.Data.Entities;
using ITC.Data.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace ITC.Core.Service
{
    public class RegistrationService : IRegistrationService
    {
        private readonly ITCDBContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;
        private readonly string _storageConnectionString;
        private readonly string _storageContainerName;
        private readonly ILogger<AzureBlobStorageService> _logger;

        public RegistrationService(ITCDBContext context, IMapper mapper
                                , IUserContextService userContextService
                                , IConfiguration configuration, ILogger<AzureBlobStorageService> logger)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
            _logger = logger;
            _storageConnectionString = configuration["BlobConnectionString"];
            _storageContainerName = configuration["BlobContainerNameImage"];
        }

        public async Task<ResultModel> CreateNew(CreateRegistrationModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var resId = Guid.NewGuid();
                var res = new Registration
                {
                    Id = resId,
                    ParentRegistrationsId = model.ParentId,
                    Title = model.Title,
                    NumberPassPort = model.NumberPassPort,
                    PassportImageUrl = null,
                    UrlImageBill = null,
                    ScocialLink = model.ScocialLink,
                    DateOfBirth = model.DateOfBirth,
                    YourEmail = model.YourEmail,
                    Status = true,
                    Creator = _userContextService.FullName,
                    DateOpenRegis = model.DateOpenRegis,
                    DateCloseRegis = model.DateCloseRegis,
                    DateExpired = model.DateExpired,
                    ProjectId = model.ProjectId,
                    StudentId = model.StudentId
                };
                if (model.ParentId == null)
                {
                    if (model.AddMoreOptinal != null)
                    {
                        foreach (var item in model.AddMoreOptinal)
                        {
                            var addOption = new RegistrationAddOn
                            {
                                Id = Guid.NewGuid(),
                                RegistrationId = res.Id,
                                Question = item,
                                Answer = null,
                            };
                            await _context.RegistrationAddOn.AddAsync(addOption);
                        }
                    }
                }
                if (model.ParentId != null)
                {
                    var checkParent = await _context.Registration.Where(x => x.Id == model.ParentId).FirstOrDefaultAsync();
                    if (checkParent != null)
                    {
                        var checkListParent = await _context.RegistrationAddOn.Where(x => x.RegistrationId == checkParent.Id).ToListAsync();
                        foreach (var item in checkListParent)
                        {
                            var addOption = new RegistrationAddOn
                            {
                                Id = Guid.NewGuid(),
                                RegistrationId = resId,
                                Question = item.Question,
                                Answer = null,
                            };
                            await _context.RegistrationAddOn.AddAsync(addOption);
                        }
                    }
                }
                await _context.Registration.AddAsync(res);
                await _context.SaveChangesAsync();

                result.IsSuccess = true;
                result.Code = 200;
                result.ResponseSuccess = res;
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

        public async Task<ResultModel> GetAll()
        {
            var result = new ResultModel();
            try
            {
                var res = await _context.Registration//.Include(p => p.Project)
                                                     //.Include(s => s.Student)
                                                        .ToListAsync();

                if (res == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Res Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = res;

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetDetailResByStudentId(Guid studentId)
        {
            var result = new ResultModel();
            try
            {
                var res = _context.Registration.Where(x => x.StudentId == studentId);

                if (res == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Res Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Registration.Include(p => p.Project).Include(s => s.Student).Include(x => x.RegistrationAddOn)
                                                                .Where(x => x.StudentId == studentId).ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> GetDetailResByProjectId(Guid projectId)
        {
            var result = new ResultModel();
            try
            {
                var res = _context.Registration.Where(x => x.ProjectId == projectId);

                if (res == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Res Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Registration
                                                                   //.Include(p => p.Project)
                                                                   .Include(s => s.Student)
                                                              //.Include(a => a.ChildrenRegistrations)
                                                              //.Include(x => x.RegistrationAddOn)
                                                              .Where(x => x.ProjectId == projectId && x.ParentRegistrationsId != null).ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> GetDetailResId(Guid Id)
        {
            var result = new ResultModel();
            try
            {
                var res = _context.Registration.Where(x => x.Id == Id);

                if (res == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Res Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Registration.Include(p => p.Project).Include(s => s.Student).Include(x => x.RegistrationAddOn)
                                                                .Where(x => x.Id == Id).ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> UpdateRegisByStudentId(Guid studentId, UpdateRegisInfoModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            UploadResponse response = new();
            BlobContainerClient container = new BlobContainerClient(_storageConnectionString, _storageContainerName);
            try
            {
                var regis = await _context.Registration.Where(x => x.StudentId == studentId).FirstOrDefaultAsync();
                var student = await _context.Student.Where(x => x.Id == studentId).FirstOrDefaultAsync();
                if (regis == null)
                {
                    result.Code = 400;
                    result.IsSuccess = true;
                    result.ResponseSuccess = "can't found regis";
                    return result;
                }
                if (student == null)
                {
                    result.Code = 400;
                    result.IsSuccess = true;
                    result.ResponseSuccess = "can't found student";
                    return result;
                }
                if (student != null)
                {
                    student.FullName = model.FullName;
                    if (model.PhoneNumber != null)
                    {
                        student.PhoneNumber = model.PhoneNumber;
                    }
                    if (model.RollNumber != null)
                    {
                        student.RollNumber = model.RollNumber;
                    }
                    student.MajorId = model.MajorId;
                    _context.Student.Update(student);
                }
                regis.Title = model.Title;
                regis.NumberPassPort = model.NumberPassPort;
                regis.ScocialLink = model.ScocialLink;
                regis.YourEmail = model.YourEmail;
                regis.DateOfBirth = model.DateOfBirth;
                regis.DateExpired = model.DateExpired;
                regis.ProjectId = model.ProjectId;
                if (model.PassportImageUrl != null)
                {
                    var co = from n in _context.FileTracking
                             where n.FileName + n.FileExtension == model.PassportImageUrl.FileName
                             select n;
                    int num = co.Count();
                    await container.CreateIfNotExistsAsync();
                    var fileextension = Path.GetExtension(model.PassportImageUrl?.FileName);
                    var filename = model.PassportImageUrl?.FileName;
                    var newFileName = model.PassportImageUrl?.FileName.Split(".").First() + " " + "(" + (num + 1) + ")" + "." + model.PassportImageUrl?.FileName.Split(".").Last();
                    BlobClient client = container.GetBlobClient(filename);
                    if (client.Exists())
                    {
                        client = container.GetBlobClient(newFileName);
                    }
                    await using (Stream? data = model.PassportImageUrl?.OpenReadStream())
                    {
                        await client.UploadAsync(data);
                    }
                    response.Status = $"File {model.PassportImageUrl?.FileName} Uploaded Successfully";
                    response.Error = false;
                    response.Uri = client.Uri.AbsoluteUri;
                    response.Name = client.Name;
                    var FileTracking = new FileTracking
                    {
                        TraceId = studentId,
                        FileName = response.Name,
                        FileExtension = model.PassportImageUrl?.FileName.Split(".").Last(),
                        FileUrl = response.Uri,
                        Owner = _userContextService.FullName,
                        DateUpLoad = DateTime.Now
                    };
                    await _context.FileTracking.AddAsync(FileTracking);
                    regis.PassportImageUrl = response.Uri;
                }
                if (model.UrlImageBill != null)
                {
                    var co = from n in _context.FileTracking
                             where n.FileName + n.FileExtension == model.UrlImageBill.FileName
                             select n;
                    int num = co.Count();
                    await container.CreateIfNotExistsAsync();
                    var fileextension = Path.GetExtension(model.UrlImageBill?.FileName);
                    var filename = model.UrlImageBill?.FileName;
                    var newFileName = model.UrlImageBill?.FileName.Split(".").First() + " " + "(" + (num + 1) + ")" + "." + model.UrlImageBill?.FileName.Split(".").Last();
                    BlobClient client = container.GetBlobClient(filename);
                    if (client.Exists())
                    {
                        client = container.GetBlobClient(newFileName);
                    }
                    await using (Stream? data = model.UrlImageBill?.OpenReadStream())
                    {
                        await client.UploadAsync(data);
                    }
                    response.Status = $"File {model.UrlImageBill?.FileName} Uploaded Successfully";
                    response.Error = false;
                    response.Uri = client.Uri.AbsoluteUri;
                    response.Name = client.Name;
                    var FileTracking = new FileTracking
                    {
                        TraceId = studentId,
                        FileName = response.Name,
                        FileExtension = model.UrlImageBill?.FileName.Split(".").Last(),
                        FileUrl = response.Uri,
                        Owner = _userContextService.FullName,
                        DateUpLoad = DateTime.Now
                    };
                    await _context.FileTracking.AddAsync(FileTracking);
                    regis.UrlImageBill = response.Uri;
                }

                _context.Registration.Update(regis);
                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Code = 200;
                await transaction.CommitAsync();
                result.ResponseSuccess = regis;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }



        public async Task<ResultModel> UpdateRegisId(Guid Id, UpdateRegisModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            UploadResponse response = new();
            BlobContainerClient container = new BlobContainerClient(_storageConnectionString, _storageContainerName);
            try
            {
                var regis = await _context.Registration.Where(x => x.Id == Id).FirstOrDefaultAsync();

                if (regis == null)
                {
                    result.Code = 400;
                    result.IsSuccess = true;
                    result.ResponseSuccess = "can't found regis";
                    return result;
                }
                regis.Title = model.Title;
                regis.NumberPassPort = model.NumberPassPort;
                regis.ScocialLink = model.ScocialLink;
                regis.YourEmail = model.YourEmail;
                regis.DateOfBirth = model.DateOfBirth;
                regis.DateExpired = model.DateExpired;
                regis.DateOpenRegis = model.DateOpenRegis;
                regis.DateCloseRegis = model.DateCloseRegis;
                regis.ProjectId = model.ProjectId;

                if (model.PassportImageUrl != null && regis.StudentId != null)
                {
                    var co = from n in _context.FileTracking
                             where n.FileName + n.FileExtension == model.PassportImageUrl.FileName
                             select n;
                    int num = co.Count();
                    await container.CreateIfNotExistsAsync();
                    var fileextension = Path.GetExtension(model.PassportImageUrl?.FileName);
                    var filename = model.PassportImageUrl?.FileName;
                    var newFileName = model.PassportImageUrl?.FileName.Split(".").First() + " " + "(" + (num + 1) + ")" + "." + model.PassportImageUrl?.FileName.Split(".").Last();
                    BlobClient client = container.GetBlobClient(filename);
                    if (client.Exists())
                    {
                        client = container.GetBlobClient(newFileName);
                    }
                    await using (Stream? data = model.PassportImageUrl?.OpenReadStream())
                    {
                        await client.UploadAsync(data);
                    }
                    response.Status = $"File {model.PassportImageUrl?.FileName} Uploaded Successfully";
                    response.Error = false;
                    response.Uri = client.Uri.AbsoluteUri;
                    response.Name = client.Name;
                    var FileTracking = new FileTracking
                    {
                        TraceId = regis.StudentId,
                        FileName = response.Name,
                        FileExtension = model.PassportImageUrl?.FileName.Split(".").Last(),
                        FileUrl = response.Uri,
                        Owner = _userContextService.FullName,
                        DateUpLoad = DateTime.Now
                    };
                    await _context.FileTracking.AddAsync(FileTracking);
                    regis.PassportImageUrl = response.Uri;
                }
                if (model.UrlImageBill != null && regis.StudentId != null)
                {
                    var co = from n in _context.FileTracking
                             where n.FileName + n.FileExtension == model.UrlImageBill.FileName
                             select n;
                    int num = co.Count();
                    await container.CreateIfNotExistsAsync();
                    var fileextension = Path.GetExtension(model.UrlImageBill?.FileName);
                    var filename = model.UrlImageBill?.FileName;
                    var newFileName = model.UrlImageBill?.FileName.Split(".").First() + " " + "(" + (num + 1) + ")" + "." + model.UrlImageBill?.FileName.Split(".").Last();
                    BlobClient client = container.GetBlobClient(filename);
                    if (client.Exists())
                    {
                        client = container.GetBlobClient(newFileName);
                    }
                    await using (Stream? data = model.UrlImageBill?.OpenReadStream())
                    {
                        await client.UploadAsync(data);
                    }
                    response.Status = $"File {model.UrlImageBill?.FileName} Uploaded Successfully";
                    response.Error = false;
                    response.Uri = client.Uri.AbsoluteUri;
                    response.Name = client.Name;
                    var FileTracking = new FileTracking
                    {
                        TraceId = regis.StudentId,
                        FileName = response.Name,
                        FileExtension = model.UrlImageBill?.FileName.Split(".").Last(),
                        FileUrl = response.Uri,
                        Owner = _userContextService.FullName,
                        DateUpLoad = DateTime.Now
                    };
                    await _context.FileTracking.AddAsync(FileTracking);
                    regis.UrlImageBill = response.Uri;
                }

                _context.Registration.Update(regis);
                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Code = 200;
                await transaction.CommitAsync();
                result.ResponseSuccess = regis;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> UpdateRegisStatus(Guid Id, bool status)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var registration = await _context.Registration.Where(x => x.Id == Id).FirstOrDefaultAsync();
                var currentDate = DateTime.Now;
                if (registration == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = $"regis with id: {Id} not existed!!";
                    return result;
                }
                registration.Status = status;
                _context.Registration.Update(registration);
                await _context.SaveChangesAsync();

                result.Code = 200;
                result.ResponseSuccess = "Succesfull";
                result.IsSuccess = true;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            await transaction.CommitAsync();
            return result;
        }
        public async Task<ResultModel> GetRootRegis()
        {
            var result = new ResultModel();
            try
            {

                List<Registration> regis = await _context.Registration
                                                    .Include(x => x.Project).Include(c => c.ChildrenRegistrations)
                                                    .AsNoTrackingWithIdentityResolution()
                                                                .ToListAsync();
                List<Registration> rootRegis = regis
                            .Where(c => c.ParentRegistrationsId == null)
                                .AsParallel()
                                    .ToList();

                result.IsSuccess = true;
                result.Code = 200;
                result.ResponseSuccess = rootRegis;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetChildReg(Guid parentId)
        {
            var result = new ResultModel();
            try
            {
                var rootRes = _context.Registration.Include(x => x.Student)
                                                    .Include(x => x.Project).Include(c => c.ChildrenRegistrations)
                            .Where(c => c.ParentRegistrationsId == parentId)
                                .AsParallel()
                                    .ToList();

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = rootRes;

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> CreateNewOptional(CreateOptionalModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                foreach (var item in model.Question)
                {
                    var optional = new RegistrationAddOn
                    {
                        Id = Guid.NewGuid(),
                        RegistrationId = model.RegistrationsId,
                        Question = item,
                    };
                    await _context.RegistrationAddOn.AddAsync(optional);
                }

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


        public async Task<ResultModel> DeleteOption(Guid resId, Guid optionId)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var op = _context.RegistrationAddOn.FirstOrDefault(x => x.RegistrationId == resId & x.Id == optionId);
                if (op == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = "optional not found ";
                    return result;
                }
                _context.RegistrationAddOn.Remove(op);
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

        public async Task<ResultModel> UpdateQuestion(UpdateQuestionModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var op = await _context.RegistrationAddOn.Where(x => x.RegistrationId == model.RegistrationId & x.Id == model.Id).FirstOrDefaultAsync();
                if (op == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = "question not found ";
                    return result;
                }
                op.Question = model.Question;
                _context.RegistrationAddOn.Update(op);
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

        public async Task<ResultModel> UpdateAnswer(UpdateAnswerModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var op = await _context.RegistrationAddOn.Where(x => x.RegistrationId == model.RegistrationId & x.Id == model.Id).FirstOrDefaultAsync();
                if (op == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = "Question not found ";
                    return result;
                }
                op.Answer = model.Answer;
                _context.RegistrationAddOn.Update(op);
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

