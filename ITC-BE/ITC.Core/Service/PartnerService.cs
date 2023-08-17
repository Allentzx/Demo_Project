using System;
using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using Irony.Parsing;
using ITC.Core.Data;
using ITC.Core.Interface;
using ITC.Core.Model;
using ITC.Data.Entities;
using ITC.Data.Enum;
using Microsoft.EntityFrameworkCore;

namespace ITC.Core.Service
{
    public class PartnerService : IPartnerService
    {

        private readonly ITCDBContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public PartnerService(ITCDBContext context, IMapper mapper, IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }
        public async Task<ResultModel> CreatePartner(PartnerCreateModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var p = _context.Partner.FirstOrDefault(x=> x.Name == model.Name);
                var header = _context.Staff.FirstOrDefault(x => x.IsHeadOfDepartMent == true && _userContextService.UserID == x.AccountId);
                var admin = _context.Account.FirstOrDefault(x => x.AccountId == _userContextService.UserID && x.Role == RoleEnum.Admin);
                if (admin != null || header != null)
                {
                    if (p != null)
                    {
                        result.Code = 400;
                        result.IsSuccess = false;
                        result.ResponseSuccess = $"Any Partners Name is existed!";
                        return result;
                    }
                    var partner = new Partner
                    {
                        Id = Guid.NewGuid(),
                        Name = model.Name,
                        Local = model.Local,
                        Note = model.Note,
                        Status = model.Status,
                    };

                    await _context.Partner.AddAsync(partner);
                    await _context.SaveChangesAsync();
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.ResponseSuccess = partner;
                    await transaction.CommitAsync();

                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.ResponseSuccess = "Partner must be create by HOD IC or admin";
                }
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> GetPartner()
        {
            var result = new ResultModel();
            try
            {
                var p = _context.Partner;

                if (p == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Partner Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Partner.Include(x => x.Campuses).Include(av => av.Deputies)
                                                    .Include(x => x.Projects).ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetDetailPartner(Guid Id)
        {
            var result = new ResultModel();
            try
            {
                var p = _context.Partner.Where(x => x.Id == Id);


                if (p == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Partner Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Partner.Include(x => x.Campuses).Include(av => av.Deputies)
                                                    .Include(x => x.Projects).Where(x => x.Id == Id).ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }
        public async Task<ResultModel> UpdatePartner(Guid Id, PartnerUpdateModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var m = await _context.Partner.FindAsync(Id);
                if (m == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Partner Not Found!";
                    return result;
                }

                m.Name = model.Name;
                m.Local = model.Local;
                m.Note = model.Note;
                m.Status = model.Status;
                _context.Partner.Update(m);
                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Code = 200;
                await transaction.CommitAsync();
                result.ResponseSuccess = await _context.Partner.Where(x => x.Id == Id)
                                                               .Include(x => x.Deputies).ToListAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> DisablePartner(Guid id)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var f = await _context.Partner.FirstOrDefaultAsync(x => x.Id == id);
                var projectstatus = await _context.Project.Where(x => x.ProjectStatus == ProjectStatusEnum.Canceled
                || x.ProjectStatus == ProjectStatusEnum.InActive && x.PartnerId == id).FirstOrDefaultAsync();
                if (projectstatus == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = $"partner with id: {f.Name} can't disalbe when project running!!";
                    return result;
                }
                if (f == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = $"Partner with id: {id} not existed!!";
                    return result;
                }

                f.Status = false;
                _context.Partner.Update(f);
                await _context.SaveChangesAsync();

                result.Code = 200;
                result.ResponseSuccess = "Partner is disable";
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

        public async Task<ResultModel> SearchPartner(string keyword)
        {
            var result = new ResultModel();
            try
            {
                var partner = await _context.Partner.Where(x => x.Name.Contains(keyword)).ToListAsync();
                if (!partner.Any())
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = "not found";
                    return result;
                }

                result.IsSuccess = true;
                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = partner;

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

