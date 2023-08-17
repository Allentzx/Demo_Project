using System;
using ITC.Core.Model;

namespace ITC.Core.Interface
{
	public interface IPartnerService
	{
        Task<ResultModel> GetPartner();
        Task<ResultModel> GetDetailPartner(Guid Id);
        Task<ResultModel> CreatePartner(PartnerCreateModel model);
        Task<ResultModel> DisablePartner(Guid id);
        Task<ResultModel> UpdatePartner(Guid Id, PartnerUpdateModel model);
        Task<ResultModel> SearchPartner(string keyword);
    }
}

