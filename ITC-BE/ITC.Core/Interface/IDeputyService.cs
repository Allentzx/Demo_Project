using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITC.Core.Model;

namespace ITC.Core.Interface
{
    public interface IDeputyService
    {
        Task<ResultModel> CreateDeputy(CreateDeputyDTO model);
        Task<ResultModel> GetAllDeputy();
        Task<ResultModel> UpdateDeputy(Guid Id, UpdateDeputyDTO model);
    }
}
