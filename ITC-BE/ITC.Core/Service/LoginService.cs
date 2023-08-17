using System;
using AutoMapper;
using Google.Apis.Auth;
using ITC.Core.Common;
using ITC.Core.Data;
using ITC.Core.Interface;
using ITC.Core.Model;
using ITC.Data.Entities;
using ITC.Data.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ITC.Core.Service
{
    public class LoginService : ILoginService
    {
        private readonly ITCDBContext _context;
        private readonly IMapper _mapper;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly string _config;

        public LoginService(ITCDBContext context, IMapper mapper, IJwtTokenService jwtTokenService, IConfiguration config)
        {
            _context = context;
            _mapper = mapper;
            _jwtTokenService = jwtTokenService;
            _config = config["FcmNotification:FirebaseToken"];
        }

        public async Task<ResultModel> AuthenticateUser(string Email, string password)
        {
            var result = new ResultModel();
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = _context.Account
                           .Include(e => e.Staff)
                           .Include(e => e.Deputy)
                           .FirstOrDefault(e => e.Email.Equals(Email));

                if (user != null)
                {
                    if (!Encryption.EncodeTo64(password).Equals(user.Password)) user = null;
                }
                if(user == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = "Wrong email or password!";
                    return result;
                }
                var staff = await _context.Staff.Where(x => x.AccountId == user.AccountId).FirstOrDefaultAsync();
                
                if (staff != null)
                {
                    result.ResponseSuccess = new LoginReponse
                    {
                        Id = user!.AccountId,
                        Email = user.Email,
                        FullName = user.FullName,
                        Role = user.Role,
                        UrlAvatar = user.UrlAvatar,
                        Staff = new StaffModel
                        {
                            Id = staff.Id,
                            StaffCode = staff.StaffCode,
                            IsHeadOfDepartMent = staff.IsHeadOfDepartMent
                        },
                        Status = user.Status,
                        AccountToken = _jwtTokenService.GenerateTokenUser(user)
                    };
                }
                if (staff == null)
                {
                    result.ResponseSuccess = new LoginReponse
                    {
                        Id = user!.AccountId,
                        Email = user.Email,
                        FullName = user.FullName,
                        Role = user.Role,
                        UrlAvatar = user.UrlAvatar,
                        Staff = null,
                        Status = user.Status,
                        AccountToken = _jwtTokenService.GenerateTokenUser(user)
                    };
                }
                result.IsSuccess = true;
                result.Code = 200;
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }


        public async Task<ResultModel> GoogleAuthenticate(GoogleJsonWebSignature.Payload user)
        {
            var result = new ResultModel();
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var account = await _context.Account.FirstOrDefaultAsync(x => x.Email == user.Email);

                if (account is null)
                {
                    var newAccount = new Account
                    {
                        AccountId = Guid.NewGuid(),
                        Email = user.Email,
                        Password = Encryption.EncodeTo64("123456"),
                        FullName = user.Name,
                        Role = RoleEnum.Staff,
                        Status = true,
                        //FireBaseToken = _config, 
                        UrlAvatar = user.Picture
                    };
                    await _context.Account.AddAsync(newAccount);
                    await _context.SaveChangesAsync();
                    account = newAccount;
                    var staff = await _context.Staff.Where(x => x.AccountId == account!.AccountId).FirstOrDefaultAsync();
                    if (staff == null)
                    {
                        var newStaff = new Staff
                        {
                            Id = Guid.NewGuid(),
                            IsHeadOfDepartMent = false,
                            StaffCode = account.Email.Split("@").First(),
                            AccountId = account.AccountId
                        };
                        staff = newStaff;
                        await _context.Staff.AddAsync(staff);
                        await _context.SaveChangesAsync();
                    }
                }
                var staff1 = await _context.Staff.FirstOrDefaultAsync(x => x.AccountId == account.AccountId);
                
                if (staff1 == null)
                {
                    result.ResponseSuccess = new LoginReponse
                    {
                        Id = account.AccountId,
                        Email = account.Email,
                        FullName = account.FullName,
                        Role = account.Role,
                        UrlAvatar = account.UrlAvatar,
                        Staff = null,
                        Status = account.Status,
                        AccountToken = _jwtTokenService.GenerateTokenUser(account)
                    };
                }
                if (staff1 != null)
                {
                    result.ResponseSuccess = new LoginReponse
                    {
                        Id = account.AccountId,
                        Email = account.Email,
                        FullName = account.FullName,
                        Role = account.Role,
                        UrlAvatar = account.UrlAvatar,
                        Status = account.Status,
                        Staff = new StaffModel
                        {
                            Id = staff1!.Id,
                            StaffCode = staff1!.StaffCode,
                            IsHeadOfDepartMent = staff1.IsHeadOfDepartMent
                        },
                        AccountToken = _jwtTokenService.GenerateTokenUser(account)
                    };
                }
                result.IsSuccess = true;
                result.Code = 200;
                await transaction.CommitAsync();

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

