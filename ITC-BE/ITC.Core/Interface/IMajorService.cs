using System;
using ITC.Core.Model;

namespace ITC.Core.Interface
{
    public interface IMajorService
    {
        Task<ResultModel> CreateMajor(CreateMajorModel model);
        Task<ResultModel> GetAllMajor();
        Task<ResultModel> UpdateMajor(Guid Id, UpdateMajorModel model);
        Task<ResultModel> GetDetailMajor(Guid Id);
        Task<ResultModel> DeleteMajor(Guid Id);
        Task<ResultModel> SearchMajor(string keyword);
    }
}

