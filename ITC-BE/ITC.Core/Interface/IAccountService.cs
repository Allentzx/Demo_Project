using ITC.Core.Model;
using ITC.Data.Enum;

namespace ITC.Core.Interface
{
    public interface IAccountService
    {
        Task<ResultModel> CreateNewAccount(CreateNewAccountModel model);
        Task<ResultModel> GetAccountById(Guid Id);
        Task<ResultModel> GetAllAccount();
        Task<ResultModel> UpdateAccount(Guid Id, UpdateAccountModel model);
        Task<ResultModel> ChangePassword(ChangePasswordModel model);
        Task<ResultModel> ChangeStatusAccount(string email, bool Status);
        Task<ResultModel> UpdateRole(string email, RoleEnum roleEnum);
    }
}

