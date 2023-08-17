using System;
using AutoMapper;
using ITC.Core.Data;
using ITC.Core.Interface;
using ITC.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace ITC.Core.Service
{
    public class NotificationService : INotificationService
    {
        private readonly ITCDBContext _context;
        private readonly IMapper _mapper;
        public NotificationService(ITCDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResultModel> GetNotificationByAccountId(Guid accountId)
        {
            var result = new ResultModel();
            try
            {
                var noti = _context.Notification.Where(x => x.AccountId == accountId);

                if (noti == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Notification Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Notification.Where(x => x.AccountId == accountId).ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> UpdateStatusNotificationByNotiId(Guid notiId)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var f = await _context.Notification.FirstOrDefaultAsync(x => x.Id == notiId);

                if (f == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = $"noti with id: {notiId} not existed!!";
                    return result;
                }
                if (f.Read == true)
                {
                    f.Read = false;
                }
                else
                    f.Read = true;
                _context.Notification.Update(f);
                await _context.SaveChangesAsync();

                result.Code = 200;
                result.ResponseSuccess = "Phase is disable";
                result.IsSuccess = true;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            await transaction.CommitAsync();
            return result;
        }
    }
}

