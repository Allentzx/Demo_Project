using System;
using ITC.Core.Interface;
using ITC.Core.Model;
using ITC.Data.Enum;
using Microsoft.AspNetCore.Mvc;

namespace ITC_BE.Controller
{
    [Route("api/v1/account")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _service;
        public AccountController(IAccountService service)
        {
            _service = service;
        }

        /// <summary>
        /// Create a new Account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateNewAccount(CreateNewAccountModel model)
        {
            var result = await _service.CreateNewAccount(model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// Get all Account
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllAccount()
        {
            var result = await _service.GetAllAccount();

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// Get a detail Account by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("getDetail/{Id}")]
        public async Task<IActionResult> GetDetailAccount(Guid Id)
        {
            var result = await _service.GetAccountById(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Update Account info
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("update/{Id}")]
        public async Task<IActionResult> Update(Guid Id, UpdateAccountModel model)
        {
            var result = await _service.UpdateAccount(Id, model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Change Password
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var result = await _service.ChangePassword(model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Change Status Account
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPut("changeStatusAccount/{email}")]
        public async Task<IActionResult> ChangeStatusAccount(string email, bool Status)
        {
            var result = await _service.ChangeStatusAccount(email, Status);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Disable Account by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPut("ChangeRole/{email}")]
        public async Task<IActionResult> ChangeRole(string email, RoleEnum roleEnum)
        {
            var result = await _service.UpdateRole(email, roleEnum);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

    }
}
