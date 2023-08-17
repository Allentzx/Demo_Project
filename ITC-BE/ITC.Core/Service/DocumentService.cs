using AutoMapper;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ITC.Core.Data;
using ITC.Core.Interface;
using ITC.Core.Model;
using ITC.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Core.Service
{
    public class DocumentService : IDocumentService
    {

        private readonly ITCDBContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContext;

        private readonly string _storageConnectionString;
        private readonly string _storageContainerName;
        private readonly ILogger<AzureBlobStorageService> _logger;

        public DocumentService(
            IMapper mapper, ITCDBContext context,
            IUserContextService userContextService,
            IConfiguration configuration,
            ILogger<AzureBlobStorageService> logger)
        {
            _context = context;
            _mapper = mapper;
            _userContext = userContextService;
            _logger = logger;
            _storageConnectionString = configuration["BlobConnectionString"];
            _storageContainerName = configuration["BlobContainerNameDocument"];
        }

        public async Task<ResultModel> CreateDocument(DocumentUploadApiRequest model)
        {
            var result = new ResultModel();
            DocumentResponse response = new();
            BlobContainerClient container = new BlobContainerClient(_storageConnectionString, _storageContainerName);
            await container.CreateIfNotExistsAsync();
            var transaction = _context.Database.BeginTransaction();
            BlobClient client = container.GetBlobClient(model.FormFile?.FileName);
            try
            {
                var co = from n in _context.Document
                         where n.FileName  == model.FormFile.FileName
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
                var ownId = _userContext.UserID.ToString()!;
                //add Entity
                var document = new Document
                {
                    Id = Guid.NewGuid(),
                    FileName = response.Name,
                    FileExtenstion = response.Name.Split(".").Last(),
                    MarkReportUrl = model.MarkReportUrl,
                    DateCreated = DateTime.Now,
                    Status = true,
                    Owner = _userContext.FullName,
                    ProjectId = model.ProjectId
                };


                await _context.Document.AddAsync(document);
                await _context.SaveChangesAsync();
                result.Code = 200;
                result.IsSuccess = true;
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


            await transaction.CommitAsync();
            return result;
        }


        public async Task<ResultModel> GetAllDocument()
        {
            var result = new ResultModel();
            try
            {
                var doc = _context.Document;

                if (doc == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any DocumentType Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Document.ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> GetDetailDocument(Guid id)
        {
            var result = new ResultModel();
            try
            {
                var doc = _context.Document.Where(x => x.Id == id);

                if (doc == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Document Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Document.Where(x => x.Id == id).ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }


        public async Task<ResultModel> GetDetailDocumentByProjectId(Guid projectId)
        {
            var result = new ResultModel();
            try
            {
                var doc = _context.Document.Where(x => x.ProjectId == projectId);

                if (doc == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Document Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Document.Where(x => x.ProjectId == projectId).ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<DocumentResponse> GetContent(Guid id)
        {

            var document = await _context.Document
            .FirstOrDefaultAsync(x => x.Id == id);
            BlobContainerClient client = new BlobContainerClient(_storageConnectionString, _storageContainerName);
            try
            {
                BlobClient file = client.GetBlobClient(document.FileName);

                if (await file.ExistsAsync())
                {
                    var data = await file.OpenReadAsync();
                    Stream blobContent = data;

                    var content = await file.DownloadContentAsync();

                    string name = document.FileName;
                    string contentType = content.Value.Details.ContentType;
                    return new DocumentResponse { Content = blobContent, Name = name, ContentType = contentType };
                }
            }
            catch (RequestFailedException ex)
                when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
            {
                _logger.LogError($"File {document.FileName} was not found.");
            }

            return null;
        }
    }
}
