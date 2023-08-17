using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Google.Apis.Auth;
using ITC.Core.Data;
using ITC.Core.Interface;
using ITC.Core.Model;
using ITC.Core.Utilities.Email;
using ITC.Data.Entities;
using ITC.Data.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ITC.Core.Service
{
    public class StaffService : IStaffService
    {
        private readonly ITCDBContext _context;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        private readonly IJwtTokenService _jwtTokenService;
        public StaffService(ITCDBContext context,
                            IMapper mapper,
                            IEmailService emailService,
                            IConfiguration config,
                            IJwtTokenService jwtTokenService)
        {
            _context = context;
            _mapper = mapper;
            _emailService = emailService;
            _config = config;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<ResultModel> CreateStaff(CreateStaffModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var staffCde = await _context.Account.FirstOrDefaultAsync(x => x.AccountId == model.AccountId);
                var staff = new Staff
                {
                    Id = Guid.NewGuid(),
                    StaffCode = staffCde.Email.Split("@").First(),
                    AccountId = model.AccountId,
                    IsHeadOfDepartMent = false,

                };


                await _context.Staff.AddAsync(staff);
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

        public async Task<ResultModel> GetStaffById(Guid Id)
        {
            var result = new ResultModel();
            try
            {
                var staff = _context.Staff.Include(a => a.Account).Where(x => x.Id == Id);

                if (staff == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Staff Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = staff;

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> GetStaffAccountId(Guid accountId)
        {
            var result = new ResultModel();
            try
            {
                var staff = _context.Staff.Include(a => a.Account).Where(x => x.AccountId == accountId);

                if (staff == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Staff Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = staff;

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> GetProjectByStaffId(Guid staffId)
        {
            var result = new ResultModel();
            try
            {
                var staff = await _context.JoinProject.Where(x => x.StaffId == staffId).ToListAsync();
                if (staff == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Staff Not Found!";
                    return result;
                }


                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.JoinProject.Where(x => x.StaffId == staffId)
                                                                                .Include(p => p.Project)
                                                                                .ThenInclude(emp => emp.Tasks.OrderBy(i => i.DeadLine)).AsNoTracking()
                                                                                .Include(c => c.Project.Campus)
                                                                                .Include(c => c.Project.ProjectPhase).ThenInclude(t=>t.Phase)
                                                                                .ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> GetAllStaff()
        {
            var result = new ResultModel();
            try
            {
                var staff = _context.Staff;

                if (staff == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Staff Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Staff.Include(a => a.Account).ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }


        public async Task<ResultModel> UpdateStaff(Guid Id, UpdateStaffModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var staff = _context.Staff.FirstOrDefault(x => x.Id == Id);
                if (staff == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = "Can't found Staff ";
                    return result;
                }
                staff.StaffCode = model.StaffCode;
                staff.IsHeadOfDepartMent = model.IsHeadOfDepartMent;
                staff.AccountId = model.AccountId;

                _context.Staff.Update(staff);
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

        public async Task<ResultModel> SearchStaff(string keyword)
        {
            var result = new ResultModel();
            try
            {
                var projects = await _context.Staff.Include(a => a.Account).Where(x => x.StaffCode.Contains(keyword) || x.Account.FullName.Contains(keyword) || x.Account.FullName.Contains(keyword)).ToListAsync();
                if (!projects.Any())
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = "not found";
                    return result;
                }

                result.IsSuccess = true;
                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = projects;

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
