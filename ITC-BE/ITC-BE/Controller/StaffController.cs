using AutoMapper;
using Google.Apis.Auth;
using ITC.Core.Configurations;
using ITC.Core.Data;
using ITC.Core.Interface;
using ITC.Core.Model;
using ITC.Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ITC_BE.Controller
{
    [Route("api/v1/staff")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _service;
        private readonly GoogleAuthConfiguration _googleAuthConfiguration;
        public StaffController(IStaffService service, IOptions<GoogleAuthConfiguration> options)
        {
            _service = service;
            _googleAuthConfiguration = options.Value;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateNewStaff(CreateStaffModel model)
        {
            var result = await _service.CreateStaff(model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// get a staff by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("getStaffDetail/{Id}")]
        public async Task<IActionResult> GetStaffbyid(Guid Id)
        {
            var result = await _service.GetStaffById(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }
        /// <summary>
        /// get a staff by accountId
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("getStaffAccountId/{accountId}")]
        public async Task<IActionResult> GetStaffAccountId(Guid accountId)
        {
            var result = await _service.GetStaffAccountId(accountId);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// get a project by staff by Id
        /// </summary>
        /// <param name="staffId"></param>
        /// <returns></returns>
        [HttpGet("GetProjectByStaffId/{staffId}")]
        public async Task<IActionResult> GetProjectByStaffId(Guid staffId)
        {
            var result = await _service.GetProjectByStaffId(staffId);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// get all Staff
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllStaff()
        {
            var result = await _service.GetAllStaff();

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Update Staff by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("update/{Id}")]
        public async Task<IActionResult> Update(Guid Id, UpdateStaffModel model)
        {
            var result = await _service.UpdateStaff(Id, model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// search Staff
        /// </summary>
        /// <returns></returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchStaff(string keyword)
        {
            var result = await _service.SearchStaff(keyword);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }
    }
}
