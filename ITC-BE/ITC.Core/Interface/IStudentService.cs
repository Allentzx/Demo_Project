using System;
using Google.Apis.Auth;
using ITC.Core.Model;
using ITC.Core.Model.Commom;
using Microsoft.AspNetCore.Http;

namespace ITC.Core.Interface
{
	public interface IStudentService
	{
        Task<ResultModel> GoogleAuthenticateStudent(GoogleJsonWebSignature.Payload user);
        Task<ResultModel> ImportStudent(ImportStudentModel model);
        Task<ResultModel> GetStudent();
        Task<ResultModel> GetDetailStudent(Guid Id);
        Task<ResultModel> CreateStudent(CreateStudentModel model);
        Task<ResultModel> DeleteStudent(Guid Id);
        Task<ResultModel> UpdateStudent(Guid Id, UpdateStudentModel model);
        Task<ResultModel> UploadGrading(Guid studentId, IFormFile? FormFile);
        Task<ResultModel> GetContent(Guid studentId);
        Task<FileModel> ExportStudentToExcel();
        Task<ResultModel> GetGradingStudentId(Guid studentId);
        Task<ResultModel> DeleteGrading(Guid studentId);
    }
}

