using System;
using ITC.Core.Interface;
using ITC.Data.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ITC.Core.Service
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;
        public static readonly Guid SYSTEM_ACCOUNT_ID = new("f1eaca5e-fad5-1eaf-fa11-babb1ed0b0e5");
        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(params Claim[] claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));


            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenClaims = claims.ToList();

            // Add default system account id
            if (tokenClaims.All(cl => cl.Type != ClaimTypes.NameIdentifier))
            {
                tokenClaims.Add(new Claim(ClaimTypes.NameIdentifier, SYSTEM_ACCOUNT_ID.ToString()));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(tokenClaims),
                Issuer = _configuration["JWT:Issuer"],
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateTokenUser(Account account)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));  //Mã hóa Key trong appsetting.json
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);   //Giải thuật encode: HMAC SHA-256
            var claims = new List<Claim>
            {
                new Claim("Email", account.Email ?? ""),
                new Claim("FullName", account.FullName ?? ""),
                new Claim("UserId", account.AccountId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())  //JWT ID
            };

            /*foreach (var role in account.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }*/

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddDays(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateTokenStudent(Student student)
        {
           var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));  //Mã hóa Key trong appsetting.json
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);   //Giải thuật encode: HMAC SHA-256
            var claims = new List<Claim>
            {
                new Claim("Email", student.Email ?? ""),
                new Claim("FullName", student.FullName ?? ""),
                new Claim("UserId", student.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())  //JWT ID
            };

            /*foreach (var role in account.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }*/

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddDays(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
