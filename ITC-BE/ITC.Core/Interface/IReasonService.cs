using ITC.Core.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Core.Interface
{
    public interface IReasonService
    {
        Task<ResultModel> CreateReason( CreateReasonModel model);
        Task<ResultModel> GetAllReason();
        Task<ResultModel> GetDetailReason(Guid Id);


    }
}
