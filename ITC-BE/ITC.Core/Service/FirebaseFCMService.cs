using System;
using ITC.Core.Interface;
using ITC.Data.Entities;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using ITC.Core.Data;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Presentation;
using System.Diagnostics.Contracts;
using ITC.Core.Model;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace ITC.Core.Service
{
    public class FirebaseFCMService : IFirebaseFCMService
    {
        private readonly ITCDBContext _context;
        private readonly INotificationService _serNoti;

        private const string ServerKeyWeb = "AAAAPv_4Up0:APA91bEHpjlUzWfwjjHZh3Obu1fS-pTw9HtuNUqHmB6j7vwK4G3MY3voBU_tk-kW4_s6o6tVihkgDqB6SHXoNyT_SWR30eYI19WEgDE_3kiRoKwREyGplrZXjhyYGyYAYy7RVVRbnTSl";
        private const string SenderIdWeb = "270582436509";

        public FirebaseFCMService(INotificationService serNoti, ITCDBContext context)
        {
            _serNoti = serNoti;
            _context = context;
        }

        public async Task<ResultModel> SaveToken(Guid accountId, string token)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var account = await _context.Account.Where(x=>x.AccountId==accountId).FirstOrDefaultAsync();
                if (account == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = "Can't found Account";
                    return result;
                }
                account.FireBaseToken = token;
                _context.Account.Update(account);
                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Code = 200;
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task PushNotification(Guid? notiId,Guid? senderAccountID, Guid? accountId, Guid? CommentId, Guid? TaskId, Guid? ProjectId, string? bodyCustom)
        {
            if (accountId != null)
            {
                var tokenDevice = await _context.Account.Where(x => x.AccountId == accountId).FirstOrDefaultAsync();
                if (tokenDevice.FireBaseToken != null)
                {
                    //Config request
                    WebRequest webRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    webRequest.Method = "POST";
                    webRequest.UseDefaultCredentials = true;
                    webRequest.ContentType = "application/json";


                    webRequest.Headers.Add(string.Format("Authorization: key={0}", ServerKeyWeb));
                    webRequest.Headers.Add(string.Format("Sender: id={0}", SenderIdWeb));

                    var payload = new object();
                    var noti = await _context.Notification.Where(x=>x.AccountId == accountId).FirstOrDefaultAsync();
                    payload = new
                    {
                       to = tokenDevice.FireBaseToken,
                       notification = new
                       {
                           title = noti.Title,
                           body = noti.Body,
                       },
                       data = new
                       {
                           notiId,
                           CommentId,
                           TaskId,
                           ProjectId
                       }
                    };

                    // Parse Json
                    var postData = JsonConvert.SerializeObject(payload).ToString();
                    Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                    webRequest.ContentLength = byteArray.Length;
                    using (Stream dataStream = webRequest.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);
                        using (WebResponse tResponse = webRequest.GetResponse())
                        {
                            using (Stream dataStreamResponse = tResponse.GetResponseStream())
                            {
                                if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                    {
                                        string sResponseFromServer = tReader.ReadToEnd();
                                        Console.WriteLine(sResponseFromServer);
                                    }
                            }
                        }
                    }
                }
            }
        }

    }
}

