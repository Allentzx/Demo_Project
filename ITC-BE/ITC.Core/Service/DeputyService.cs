using System;
using AutoMapper;
using ITC.Core.Common;
using ITC.Core.Data;
using ITC.Core.Interface;
using ITC.Core.Model;
using ITC.Core.Utilities.Email;
using ITC.Data.Entities;
using ITC.Data.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ITC.Core.Service
{
	public class DeputyService : IDeputyService
	{
        private readonly ITCDBContext _context;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        private readonly IUserContextService _userContextService;
        public DeputyService(ITCDBContext context, IMapper mapper, IEmailService emailService,
                            IConfiguration config, IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _emailService = emailService;
            _config = config;
            _userContextService = userContextService;
        }

        public async Task<ResultModel> CreateDeputy(CreateDeputyDTO model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var Ava = _context.Deputy.FirstOrDefault(x => x.AccountId == model.AccountId);
                var account = _context.Account.FirstOrDefault(x => x.AccountId == model.AccountId && x.Role == RoleEnum.Partner);
                var partner = _context.Partner.FirstOrDefault(x => x.Id == model.PartnerId);

                //var status = _context.Account.FirstOrDefault(x => x.Status == true && x.Role == RoleEnum.Partner );
                
                //if (status != null)
                //{
                //    status.Status = false;
                //    _context.SaveChanges();
                //}
                if (Ava != null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = "Deputy Existed!";
                    return result;
                }

                if (account == null && partner == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = "Account and Partner must be Existed!";
                    return result;
                }
                if (account != null  && partner != null )
                { 
                    var deputy = new Deputy
                    {
                        Id = Guid.NewGuid(),
                        AccountId = model.AccountId,
                        PartnerId = model.PartnerId
                    };
                    account.Status = true;
                    
                    _emailService.Send(_config["Emails:SmtpUser"], account.Email, Decryption.DecodeFrom64(account.Password), account.FullName);

                    await _context.Deputy.AddAsync(deputy);
                }
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

        public async Task<ResultModel> GetAvaById(Guid Id)
        {
            var result = new ResultModel();
            try
            {
                var Deputys = _context.Deputy.Where(x => x.AccountId == Id);

                if (Deputys == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Deputys Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _mapper.ProjectTo<DeputyViewModel>(Deputys).FirstOrDefaultAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> GetAllDeputy()
        {
            var result = new ResultModel();
            try
            {
                var ava = _context.Deputy.Include(x=> x.Account);

                if (ava == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Deputy Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _mapper.ProjectTo<DeputyViewModel>(ava).ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }


        public async Task<ResultModel> UpdateDeputy(Guid Id, UpdateDeputyDTO model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var deputy = await _context.Deputy.FindAsync(Id);
                if (deputy == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = "Can't found deputy";
                    return result;
                }
                //var status = _context.Account.SingleOrDefault(x => x.Status == true);
                //if (status != null)
                //{
                //    status.Status = false;
                //    _context.SaveChanges();
                //}
                deputy.PartnerId = model.PartnerId;
                deputy.AccountId = model.AccountId;
                
                _context.Deputy.Update(deputy);
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

    }
}

