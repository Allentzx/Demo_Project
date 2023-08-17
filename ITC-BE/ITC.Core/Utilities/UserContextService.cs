using System;
using Microsoft.AspNetCore.Http;
using ITC.Core.Interface;

namespace ITC.Core.Utilities
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public UserContextService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public Guid? UserID =>
            Guid.TryParse(
                _contextAccessor.HttpContext?.User.Claims.FirstOrDefault(cl => cl.Type == "UserId")
                    ?.Value, out var id)
                ? id
                : null;

        public string? Username =>
            _contextAccessor.HttpContext?.User.Claims.FirstOrDefault(cl => cl.Type == "UserName")?.Value ?? null;

        public string? Email =>
            _contextAccessor.HttpContext?.User.Claims.FirstOrDefault(cl => cl.Type == "Email")?.Value ?? null;
        public string? FullName => _contextAccessor.HttpContext?.User.Claims.FirstOrDefault(cl => cl.Type == "FullName")?.Value ?? null;
        public bool IsAuthenticated => _contextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}

