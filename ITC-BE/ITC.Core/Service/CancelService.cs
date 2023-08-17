using AutoMapper;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ITC.Core.Data;
using ITC.Core.Interface;
using ITC.Core.Model;
using ITC.Core.Model.Commom;
using ITC.Core.Utilities;
using ITC.Core.Utilities.Exceptions;
using ITC.Core.Utilities.FileUti;
using ITC.Data.Entities;
using ITC.Data.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
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
    public class CancelService : ICancelService
    {
        private readonly ITCDBContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;
        private readonly string _storageConnectionString;
        private readonly string _storageContainerName;
        private readonly ILogger<AzureBlobStorageService> _logger;

        public CancelService(ITCDBContext context, IMapper mapper, IUserContextService userContextService
                                                        , IConfiguration configuration, ILogger<AzureBlobStorageService> logger)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
            _logger = logger;
            _storageConnectionString = configuration["BlobConnectionString"];
            _storageContainerName = configuration["BlobContainerName"];

        }

        public async Task<ResultModel> CreateAsync(CancelUploadApiRequest model)
        {
            var result = new ResultModel();
            CancelResponse response = new();
            BlobContainerClient container = new BlobContainerClient(_storageConnectionString, _storageContainerName);
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var FileId = Guid.NewGuid();
                //add Entity
                var checkerHeader = await _context.Staff.Where(st => st.IsHeadOfDepartMent == true).ToListAsync();
                var checkProject = await _context.ProjectPhase.FirstOrDefaultAsync(p => p.ProjectId == model.ProjectId);
                if (checkProject != null)
                {
                    var getPhaseName = await _context.Phase.Where(mt => mt.Id == checkProject.PhaseId).FirstOrDefaultAsync();
                    if (checkerHeader != null)
                    {
                        var cancel = new CancelProject
                        {
                            Id = FileId,
                            Description = model.Description,
                            DateCreated = DateTime.Now,
                            ProjectId = model.ProjectId,
                            PhaseName = getPhaseName.PhaseName,
                            HeaderId = _userContextService.UserID,
                            HeaderName = _userContextService.FullName,

                        };

                        if (model.FormFile != null)
                        {
                            await container.CreateIfNotExistsAsync();
                            var fileextension = Path.GetExtension(model.FormFile?.FileName);
                            var filename = model.FormFile?.FileName;
                            BlobClient client = container.GetBlobClient(filename);
                            string? chek = model.FormFile?.FileName.Split(".").First();
                            var co = from n in _context.FileTracking
                                     where n.FileName.Contains(chek)
                                     select n;
                            Random random = new Random();
                            int randomNumber = random.Next(0, 1000);
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
                            var ownName = _userContextService.FullName;
                            var FileTracking = new FileTracking
                            {
                                TraceId = FileId,
                                FileName = response.Name,
                                FileExtension = model.FormFile?.FileName.Split(".").Last(),
                                FileUrl = response.Uri,
                                Owner = _userContextService.FullName
                            };
                            await _context.FileTracking.AddAsync(FileTracking);
                        }


                        await _context.CancelProject.AddAsync(cancel);
                        await _context.SaveChangesAsync();

                        result.Code = 200;
                        result.IsSuccess = true;
                        await transaction.CommitAsync();
                    }
                }
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

        public async Task<CancelResponse> GetContent(Guid id)
        {

            var FileTracking = await _context.FileTracking
            .FirstOrDefaultAsync(x => x.TraceId == id);
            BlobContainerClient client = new BlobContainerClient(_storageConnectionString, _storageContainerName);
            try
            {

                string fileName = FileTracking.FileName;
                BlobClient file = client.GetBlobClient(fileName);

                if (await file.ExistsAsync())
                {
                    var data = await file.OpenReadAsync();
                    Stream blobContent = data;

                    var content = await file.DownloadContentAsync();
                    string contentType = content.Value.Details.ContentType;
                    return new CancelResponse { Content = blobContent, Name = FileTracking.FileName, ContentType = contentType };
                }
            }
            catch (RequestFailedException ex)
                when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
            {
                _logger.LogError($"File {FileTracking.FileName} was not found.");
            }

            return null;
        }

        public async Task<ResultModel> GetProjectCancel(Guid projectId)
        {
            var result = new ResultModel();
            try
            {
                var p = _context.CancelProject.Where(x => x.ProjectId == projectId);

                if (p == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Project Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.CancelProject.Where(x => x.ProjectId == projectId).ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }
    }
}
