using System;
using ITC.Core.Model;

namespace ITC.Core.Interface
{
	public interface IFirebaseFCMService
	{
        public Task<ResultModel> SaveToken(Guid accountId, string token);
        public Task PushNotification(Guid? notiId, Guid? senderAccountID, Guid? accountId, Guid? commentId, Guid? taskId, Guid? projectId, string? b);
    }
}

