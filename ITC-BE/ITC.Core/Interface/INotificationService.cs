using System;
using ITC.Core.Model;

namespace ITC.Core.Interface
{
	public interface INotificationService
	{
        Task<ResultModel> GetNotificationByAccountId(Guid accountId);
        Task<ResultModel> UpdateStatusNotificationByNotiId(Guid notiId);
    }
}

