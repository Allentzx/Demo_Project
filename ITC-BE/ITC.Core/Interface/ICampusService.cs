using System;
using ITC.Core.Model;

namespace ITC.Core.Interface
{
	public interface ICampusService
	{
        Task<ResultModel> CreateCampus(CreateCampusModel model);
        Task<ResultModel> GetCampusById(Guid Id);
        Task<ResultModel> GetAllCampus();
        Task<ResultModel> UpdateCampus(Guid Id, UpdateCampusModel model);
        Task<ResultModel> DisableCampus(Guid Id);
    }
}

