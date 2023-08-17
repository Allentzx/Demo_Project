using System;
using Google.Apis.Auth;
using ITC.Core.Configurations;
using ITC.Core.Interface;
using ITC.Core.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ITC_BE.Controller
{
    [Route("api/v1/authen")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly GoogleAuthConfiguration _googleAuthConfiguration;
        public LoginController(ILoginService loginService, IJwtTokenService jwtTokenService,IOptions<GoogleAuthConfiguration> options)
        {
            _loginService = loginService;
            _jwtTokenService = jwtTokenService;
            _googleAuthConfiguration = options.Value;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm]LoginRequest loginRequest)
        {
            var result =  await _loginService.AuthenticateUser(loginRequest.Email, loginRequest.Password);
            if (result is { IsSuccess: true, Code: 200 }) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// login with goggle
        /// </summary>
        /// <returns></returns>
        [HttpPost("signin-google/{token}")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleAuthenticate([FromRoute] string token)
        {
            var googleUser = await GoogleJsonWebSignature.ValidateAsync(token,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _googleAuthConfiguration.ClientId }
                });

            var result = await _loginService.GoogleAuthenticate(googleUser);

            if (result is { IsSuccess: true, Code: 200 }) return Ok(result);
            return BadRequest(result);
        }
    }
}

