using System;
using Google.Apis.Auth;
using ITC.Core.Model;
using ITC.Data.Entities;

namespace ITC.Core.Interface
{
    public interface ILoginService
    {
        Task<ResultModel> AuthenticateUser(string Email, string password);

        Task<ResultModel> GoogleAuthenticate(GoogleJsonWebSignature.Payload user);
    }
}

