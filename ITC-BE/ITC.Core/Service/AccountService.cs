using System;
using System.Text.RegularExpressions;
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
    public class AccountService : IAccountService
    {
        private readonly ITCDBContext _context;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly string _config;
        private readonly IUserContextService _userContextService;
        public AccountService(ITCDBContext context, IMapper mapper, IEmailService emailService,
                            IConfiguration config, IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _emailService = emailService;
            _config = config["FcmNotification:FirebaseToken"];
            _userContextService = userContextService;
        }

        public async Task<ResultModel> CreateNewAccount(CreateNewAccountModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var checkmail = _context.Account.Where(x => x.Email == model.Email).FirstOrDefault();


                if (!Regex.IsMatch(model.Email, @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = $"Account with Email must be right format mail!!";
                    return result;
                }
                if (checkmail != null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = $"Account with Email: {model.Email}  existed!!";
                    return result;
                };
                var checkPhone = _context.Account.Where(x => x.PhoneNumber == model.PhoneNumber).FirstOrDefault();
                if (checkPhone != null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = $"Account with Phone: {model.PhoneNumber}  existed!!";
                    return result;
                };
                var account = new Account
                {
                    AccountId = Guid.NewGuid(),
                    Email = model.Email,
                    Password = Encryption.EncodeTo64("123456"),
                    FullName = model.FullName,
                    DateCreated = DateTime.Now,
                    //FireBaseToken =_config,
                    BirthDay = model.BirthDay,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    Gender = model.Gender,
                    Status = true,
                    UrlAvatar = model.UrlAvatar,
                    Role = model.Role
                };
                if (model.Role == RoleEnum.Staff)
                {
                    var staff = await _context.Staff.FirstOrDefaultAsync(x => x.AccountId == account.AccountId);
                    if (staff != null)
                    {
                        result.Code = 400;
                        result.IsSuccess = false;
                        result.ResponseSuccess = $"staff is existed!";
                        return result;
                    }
                    if (staff == null)
                    {
                        Random random = new Random();
                        int randomNumber = random.Next(0, 1000);
                        var newStaff = new Staff
                        {
                            Id = Guid.NewGuid(),
                            IsHeadOfDepartMent = false,
                            StaffCode = account.Email.Split("@").First(),
                            AccountId = account.AccountId
                        };
                        staff = newStaff;
                        await _context.Staff.AddAsync(staff);
                    }
                };
                await _context.Account.AddAsync(account);
                await _context.SaveChangesAsync();

                result.IsSuccess = true;
                result.Code = 200;
                result.ResponseSuccess = new AccountModel
                {
                    AccountId = account.AccountId,
                    Email = account.Email,
                    Password = Decryption.DecodeFrom64(account.Password),
                    FullName = account.FullName,
                    DateCreated = account.DateCreated,
                    BirthDay = account.BirthDay,
                    PhoneNumber = account.PhoneNumber,
                    Address = account.Address,
                    Gender = account.Gender,
                    Status = account.Status,
                    UrlAvatar = account.UrlAvatar,
                    Role = account.Role,
                };

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

        public async Task<ResultModel> GetAllAccount()
        {
            var result = new ResultModel();
            try
            {
                var Account = _context.Account;

                if (Account == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Partner Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Account.ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetAccountById(Guid Id)
        {
            var result = new ResultModel();
            try
            {
                var account = _context.Account.Where(x => x.AccountId == Id).FirstOrDefault();

                if (account == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Account Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = new AccountViewModel
                {
                    AccountId = account.AccountId,
                    Email = account.Email,
                    FullName = account.FullName,
                    Password = Decryption.DecodeFrom64(account.Password),
                    PhoneNumber = account.PhoneNumber,
                    UrlAvatar = account.UrlAvatar,
                    FireBaseToken = account.FireBaseToken,
                    BirthDay = account.BirthDay,
                    Address = account.Address,
                    Gender = account.Gender,
                    Role = account.Role,
                    DateCreated = account.DateCreated

                };

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> UpdateAccount(Guid Id, UpdateAccountModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var account = await _context.Account.FindAsync(Id);
                if (!Regex.IsMatch(model.Email, @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = "Email must be right format!";
                    return result;
                }
                if (account == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = "can't find account";
                    return result;
                }
                if (model.Password != model.ConfirmPassword)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = "Password Is not match";
                    return result;
                }
                account.FullName = model.FullName;
                account.Status = model.Status;
                account.Email = model.Email;
                account.Password = Encryption.EncodeTo64(model.Password);
                account.BirthDay = model.BirthDay;
                account.Address = model.Address;
                if(model.Email != null)
                {
                    account.Email = model.Email;
                }
                if(model.PhoneNumber != null)
                {
                    account.PhoneNumber = model.PhoneNumber;
                }
                account.Gender = model.Gender;
                account.UrlAvatar = model.UrlAvatar;
                account.Status = model.Status;
                account.Role = model.Role;
                _context.Account.Update(account);
                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Code = 200;
                await transaction.CommitAsync();
                result.ResponseSuccess = await _context.Account.Where(x => x.AccountId == Id).ToListAsync(); ;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> ChangePassword(ChangePasswordModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var account = await _context.Account.FirstOrDefaultAsync(x => x.Email == model.Email);
                if (account == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = "can't find account";
                    return result;
                }
                if (Decryption.DecodeFrom64(account.Password) != model.OldPassword)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = "Password Wrong";
                    return result;
                }
                if (model.NewPassword != model.ConfirmPassword)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = "Password Is not match";
                    return result;
                }
                account.Email = model.Email;
                account.Password = Encryption.EncodeTo64(model.NewPassword);
                _context.Account.Update(account);
                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Code = 200;
                await transaction.CommitAsync();
                result.ResponseSuccess = await _context.Account.Where(x => x.Email == model.Email).ToListAsync(); ;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }


        public async Task<ResultModel> ChangeStatusAccount(string email, bool Status)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var account = await _context.Account.FirstOrDefaultAsync(x => x.Email == email);
                if (account == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = $"Account with email: {email} not existed!!";
                    return result;
                }
                account.Status = Status;
                _context.Account.Update(account);
                await _context.SaveChangesAsync();

                result.Code = 200;
                result.ResponseSuccess = "Disable complete";
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


        public async Task<ResultModel> UpdateRole(string email, RoleEnum roleEnum)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var account = await _context.Account.FirstOrDefaultAsync(x => x.Email == email);

                var check = await _context.Account.FirstOrDefaultAsync(x => x.Role == RoleEnum.Admin && x.Email == _userContextService.Email);
                if (account == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = $"Account with email: {email} not existed!!";
                    return result;
                }
                if (check == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = "Only Admin can update role";
                    return result;
                }
                if (check != null)
                {
                    account.Role = roleEnum;
                    _context.Account.Update(account);
                }
                await _context.SaveChangesAsync();

                result.Code = 200;
                result.ResponseSuccess = "update role complete";
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

