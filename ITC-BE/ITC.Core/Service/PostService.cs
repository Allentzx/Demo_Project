using AutoMapper;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using ITC.Core.Data;
using ITC.Core.Interface;
using ITC.Core.Model;
using ITC.Core.Utilities;
using ITC.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Core.Service
{
    public class PostService : IPostService
    {
        private readonly ITCDBContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;
        private readonly string _storageConnectionString;
        private readonly string _storageContainerName;
        private readonly ILogger<AzureBlobStorageService> _logger;
        public PostService(ITCDBContext context, IMapper mapper
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

        public async Task<ResultModel> CreatePost(CreatePostUploadApiRequest model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            PostResponse response = new();
            BlobContainerClient container = new BlobContainerClient(_storageConnectionString, _storageContainerName);
            var traceId = Guid.NewGuid();
            try
            {
                var checkRole = _context.Staff.Where(x => x.AccountId == _userContextService.UserID).FirstOrDefault();
                var pos = _context.Post;
                if (pos == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Post Not Found!";
                    return result;
                }
                if(checkRole == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Role must be staff to create a new Post!";
                    return result;
                }

                if (model.FormFile == null)
                {
                    var post = new Post
                    {
                        Id = traceId,
                        Author = model.Author,
                        Title = model.Title,
                        SubTitle = model.SubTitle,
                        Content = model.Content,
                        DateCreated = DateTime.Now,
                        PosterUrl = null,
                        Status = true,
                        StaffId = checkRole.Id,

                    };

                    await _context.Post.AddAsync(post);
                    await _context.SaveChangesAsync();
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.ResponseSuccess = _mapper.Map<PostViewModel>(post);
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
                        TraceId = traceId,
                        FileName = response.Name,
                        FileExtension = model.FormFile?.FileName.Split(".").Last(),
                        FileUrl = response.Uri,
                        Owner = _userContextService.FullName,
                        DateUpLoad = DateTime.Now
                    };
                    var post1 = new Post
                    {
                        Id = traceId,
                        Author = model.Author,
                        Title = model.Title,
                        SubTitle = model.SubTitle,
                        Content = model.Content,
                        DateCreated = DateTime.Now,
                        PosterUrl = response.Uri,
                        Status = true,
                        StaffId = checkRole.Id,

                    };

                    await _context.Post.AddAsync(post1);
                    await _context.FileTracking.AddAsync(fileTracking);
                    await _context.SaveChangesAsync();
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.ResponseSuccess = _mapper.Map<PostViewModel>(post1);
                }


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

        public async Task<ResultModel> DeletePost(Guid id)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var po = await _context.Post.FirstOrDefaultAsync(x => x.Id == id);

                if (po == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = $"Post with id: {id} not existed!!";
                    return result;
                }

                _context.Post.Remove(po);
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



        public async Task<ResultModel> GetAllPost()
        {
            var result = new ResultModel();
            try
            {
                var po = _context.Post;

                if (po == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Post Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Post.Include(x => x.PostImages)
                .ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> GetPostById(Guid Id)
        {
            var result = new ResultModel();
            try
            {
                var p = _context.Post.Where(x => x.Id == Id);

                if (p == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Post Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Post.Include(x => x.PostImages)
                                                            .Where(x => x.Id == Id).ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> UpdatePost(Guid Id, UpdateFilePostResponse model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            PostResponse response = new();
            BlobContainerClient container = new BlobContainerClient(_storageConnectionString, _storageContainerName);
            try
            {
                var post = await _context.Post.FindAsync(Id);

                if (post == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = "Can't found Post";
                    return result;
                }
                if (model.FormFile == null)
                {
                    post.Author = model.Author;
                    post.Title = model.Title;
                    post.SubTitle = model.SubTitle;
                    post.Content = model.Content;
                    post.Status = model.Status;
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
                    await _context.FileTracking.AddAsync(fileTracking);
                    post.Author = _userContextService.FullName;
                    post.Title = model.Title;
                    post.SubTitle = model.SubTitle;
                    post.Content = model.Content;
                    post.Status = model.Status;
                    post.PosterUrl = response.Uri;
                }
                _context.Post.Update(post);
                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Code = 200;
                await transaction.CommitAsync();
                result.ResponseSuccess = _mapper.Map<PostViewModel>(post);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }


        public async Task<BlobResponseDto> UploadListImage(Guid postId, List<IFormFile> file)
        {
            BlobResponseDto response = new();
            BlobContainerClient container = new BlobContainerClient(_storageConnectionString, _storageContainerName);
            await container.CreateIfNotExistsAsync();
            foreach (var formFile in file)
            {
                try
                {
                    var post = await _context.Post.Where(x => x.Id == postId).FirstOrDefaultAsync();
                    if (post == null)
                    {
                        response.Error = false;
                        response.Status = "Can't found Post";
                        return response;
                    }
                    var fileextension = Path.GetExtension(formFile.FileName);
                    var filename = formFile?.FileName;
                    BlobClient client = container.GetBlobClient(filename);
                    var co = from n in _context.FileTracking
                             where n.FileName + n.FileExtension == formFile!.FileName
                             select n;
                    int num = co.Count();
                    var newFileName = formFile?.FileName.Split(".").First() + " " + "(" + (num + 1) + ")" + "." + formFile?.FileName.Split(".").Last();

                    if (client.Exists())
                    {
                        client = container.GetBlobClient(newFileName);
                    }

                    await using (Stream? data = formFile?.OpenReadStream())
                    {
                        await client.UploadAsync(data);
                    }

                    var fileId = Guid.NewGuid();
                    response.Blob.Uri = client.Uri.AbsoluteUri;
                    response.Blob.Name = client.Name;
                    var fileTracking = new FileTracking
                    {
                        TraceId = fileId,
                        FileName = response.Blob.Name,
                        FileExtension = formFile?.FileName.Split(".").Last(),
                        FileUrl = response.Blob.Uri,
                        Owner = _userContextService.FullName,
                        DateUpLoad = DateTime.Now
                    };
                    var image = new PostImage
                    {
                        Id =  fileId,
                        PostId = postId,
                        PostImageUrl = fileTracking.FileUrl
                    };
                    await _context.FileTracking.AddAsync(fileTracking);
                    await _context.PostImage.AddAsync(image);
                    await _context.SaveChangesAsync();
                }
                catch (RequestFailedException ex)
                when (ex.ErrorCode == BlobErrorCode.BlobAlreadyExists)
                {
                    _logger.LogError($"File with name {formFile.FileName} already exists in container. Set another name to store the file in the container: '{_storageContainerName}.'");
                    response.Status = $"File with name {formFile.FileName} already exists. Please use another name to store your file.";
                    response.Error = true;
                    return response;
                }
                catch (RequestFailedException ex)
                {
                    _logger.LogError($"Unhandled Exception. ID: {ex.StackTrace} - Message: {ex.Message}");
                    response.Status = $"Unexpected error: {ex.StackTrace}. Check log with StackTrace ID.";
                    response.Error = true;
                    return response;
                }
            }
            return response;
        }

       

        public async Task<ResultModel> DeleteImage(Guid imageId)
        {

            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();

            BlobContainerClient client = new BlobContainerClient(_storageConnectionString, _storageContainerName);
            try
            {
                var conf = await _context.FileTracking.FirstOrDefaultAsync(x => x.TraceId == imageId);
                var post = await _context.PostImage.FirstOrDefaultAsync(x => x.Id == imageId);
                var fileName = conf.FileName;
                BlobClient file = client.GetBlobClient(fileName);
                await file.DeleteAsync();
                if (post != null)
                {
                    _context.PostImage.Remove(post);
                }
                if(conf != null)
                { 
                    _context.FileTracking.Remove(conf);
                }
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
    }
}
