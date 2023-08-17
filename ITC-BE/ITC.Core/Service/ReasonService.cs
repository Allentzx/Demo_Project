using AutoMapper;
using ITC.Core.Data;
using ITC.Core.Interface;
using ITC.Core.Model;
using ITC.Core.Utilities;
using ITC.Core.Utilities.Exceptions;
using ITC.Data.Entities;
using ITC.Data.Enum;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Core.Service
{
    public class ReasonService : IReasonService
    {
        private readonly ITCDBContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public ReasonService(ITCDBContext context, IMapper mapper, IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }
        public async Task<ResultModel> CreateReason(CreateReasonModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var checkDeputi = _context.Account.Where(x => x.AccountId == _userContextService.UserID && x.Role == RoleEnum.Partner).FirstOrDefault();
                if (checkDeputi == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Role must be Partner!";
                    return result;
                }
                if (checkDeputi != null)
                {
                    var reason = new Reason
                    {
                        Id = Guid.NewGuid(),
                        ReasonContent = model.ReasonContent,
                        DateCreated = DateTime.Now,
                        SlotId = model.SlotId,
                        Deputies = _context.Deputy.Where(x => x.AccountId == _userContextService.UserID).FirstOrDefault(),
                        DeputyId = _userContextService.UserID,
                    };

                    await _context.Reason.AddAsync(reason);
                }

                await _context.SaveChangesAsync();

                result.Code = 200;
                result.IsSuccess = true;
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



        public async Task<ResultModel> GetAllReason()
        {
            var result = new ResultModel();
            try
            {
                var course = _context.Reason;

                if (course == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any course Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Reason.Include(s => s.Slot)
                                                                .Include(x => x.Deputies).ThenInclude(y => y.Partner).ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetDetailReason(Guid Id)
        {
            var result = new ResultModel();
            try
            {
                var course = _context.Course.Where(x => x.Id == Id);
                if (course == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Course Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Reason.Include(s => s.Slot)
                                                                .Include(x => x.Deputies).ThenInclude(y => y.Partner)
                                             .Where(x => x.Id == Id).ToListAsync();

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
