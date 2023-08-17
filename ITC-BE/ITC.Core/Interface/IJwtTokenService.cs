using System;
using ITC.Data.Entities;
using System.Security.Claims;

namespace ITC.Core.Interface
{
	public interface IJwtTokenService
	{
        string GenerateTokenUser(Account account);
        string GenerateTokenStudent(Student student);

        string GenerateToken(params Claim[] claims);
    }
}

