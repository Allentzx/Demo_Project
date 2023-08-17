using System;
using AutoMapper;
using ITC.Core.Data;
using ITC.Core.Interface;
using ITC.Core.Model;
using ITC.Data.Enum;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ITC.Core.Service
{
    public class AuditService : IAuditService
    {

        private readonly ITCDBContext _context;
        private readonly IMapper _mapper;
        public AuditService(ITCDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ResultModel> GetAllChangeLog()
        {
            var result = new ResultModel();
            try
            {
                var changelog = _context.AuditTrails;

                if (changelog == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Slot Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.AuditTrails.ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetDetailChangeLog(int Id)
        {
            var result = new ResultModel();
            try
            {
                var changelog = _context.AuditTrails
                                             .Where(x => x.Id == Id);

                if (changelog == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any ChangeLog Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.AuditTrails.FirstOrDefaultAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> GetChangeLogTasks()
        {
            var result = new ResultModel();
            try
            {
                var changelog = await _context.AuditTrails
                                             .Where(x => x.TableName == "Tasks").ToListAsync();


                if (changelog == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Task ChangeLog Not Found!";
                    return result;
                }
                var res = new List<AuditNewModel>();
                foreach (var item in changelog)
                {
                    var rs = new AuditNewModel()
                    {
                        UserId = item.UserId,
                        Type = item.Type,
                        TableName = item.TableName,
                        DateTime = item.DateTime,
                        // OldValues = JsonConvert.DeserializeObject(item.OldValues!),
                        NewValues = JsonConvert.DeserializeObject(item.NewValues!),
                        // AffectedColumns = item.AffectedColumns,
                        PrimaryKey = item.PrimaryKey
                    };
                    res.Add(rs);
                }
                if (res.Count != 0)
                {
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.ResponseSuccess = res;
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

    }
}

