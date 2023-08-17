using ITC.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;

namespace ITC.Core.Interface
{
    public interface IStaffService
    {
        Task<ResultModel> CreateStaff(CreateStaffModel model);
        Task<ResultModel> GetStaffById(Guid Id);
        Task<ResultModel> GetStaffAccountId(Guid accountId);
        Task<ResultModel> GetAllStaff();
        Task<ResultModel> GetProjectByStaffId(Guid staffId);
        Task<ResultModel> UpdateStaff(Guid Id, UpdateStaffModel model);
        Task<ResultModel> SearchStaff(string keyword);
    }
}
