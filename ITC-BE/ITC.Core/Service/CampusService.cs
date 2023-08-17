using System;
using AutoMapper;
using ITC.Core.Data;
using ITC.Core.Interface;
using ITC.Core.Model;
using ITC.Core.Utilities.Email;
using ITC.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ITC.Core.Service
{
    public class CampusService : ICampusService
    {
        private readonly ITCDBContext _context;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        private readonly IUserContextService _userContextService;
        public CampusService(ITCDBContext context, IMapper mapper, IEmailService emailService,
                            IConfiguration config, IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _emailService = emailService;
            _config = config;
            _userContextService = userContextService;
        }

        public async Task<ResultModel> CreateCampus(CreateCampusModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var existed = _context.Campus.FirstOrDefault(c => c.Name == model.Name);
                if (existed != null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"CampusName is Existed!";
                    return result;
                }

                var campus = new Campus
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    Address = model.Address,
                    PartnerId = model.PartnerId,
                    Status = true,
                };

                await _context.Campus.AddAsync(campus);
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

        public async Task<ResultModel> GetAllCampus()
        {
            var result = new ResultModel();
            try
            {
                var campus = _context.Campus;

                if (campus == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Partner Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Campus.ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetCampusById(Guid Id)
        {
            var result = new ResultModel();
            try
            {
                var campus = _context.Campus.Where(x => x.Id == Id);

                if (campus == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Campus Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Campus.Where(x => x.Id == Id).ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> UpdateCampus(Guid Id, UpdateCampusModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var campus = await _context.Campus.FindAsync(Id);
                if (campus == null)
                {
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.ResponseSuccess = new CampusViewModel();
                    return result;
                }

                campus.Name = model.Name;
                campus.Status = model.Status;
                campus.Address = model.Address;
                campus.PartnerId = model.PartnerId;
                _context.Campus.Update(campus);
                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Code = 200;
                await transaction.CommitAsync();
                result.ResponseSuccess = await _context.Campus.Where(x => x.Id == Id).ToListAsync(); ;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> DisableCampus(Guid Id)
        {

            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var campus = await _context.Campus.FirstOrDefaultAsync(x => x.Id == Id);
                
                if (campus == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = $"campus with id: {Id} not existed!!";
                    return result;
                }

                
                campus.Status = false;
                _context.Campus.Update(campus);
                await _context.SaveChangesAsync();

                result.Code = 200;
                result.ResponseSuccess = "Succesfull";
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

