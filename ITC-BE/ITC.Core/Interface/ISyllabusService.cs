using System;
using ITC.Core.Model;

namespace ITC.Core.Interface
{
    public interface ISyllabusService
    {
        Task<ResultModel> CreateSyllabus(CreateSyllabusModel model);
        Task<ResultModel> GetAllSyllabus();
        Task<ResultModel> GetDetailSyllabus(Guid Id);
        Task<ResultModel> UpdateSyllabus(Guid Id, UpdateSyllabusModel model);
        Task<ResultModel> UpdateSyllabusStatus(Guid Id, UpdateSyllabusStatusModel model);
        Task<ResultModel> DeleteSyllabus(Guid Id);
        Task<ResultModel> GetListSyllabusPartner(Guid? PartnerId);
        Task<ResultModel> SearchSyllabus(string keyword);
    }
}

