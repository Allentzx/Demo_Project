using System;
using ITC.Core.Model;
using ITC.Core.Model.Commom;

namespace ITC.Core.Interface
{
	public interface ICancelService
	{
        Task<ResultModel> GetProjectCancel(Guid projectId);
        Task<ResultModel> CreateAsync(CancelUploadApiRequest model);
        Task<CancelResponse> GetContent(Guid Id);
    }
}

