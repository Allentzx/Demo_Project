using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITC.Core.Interface;
using ITC.Core.Model;
using Microsoft.AspNetCore.Mvc;

namespace ITC_BE.Controller
{
    [Route("api/v1/campus")]
    public class CampusController : ControllerBase
    {
        private readonly ICampusService _service;
        public CampusController(ICampusService service)
        {
            _service = service;
        }

        /// <summary>
        /// Create a new campus
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateNewCampus(CreateCampusModel model)
        {
            var result = await _service.CreateCampus(model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// Get all Campus
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllCampus()
        {
            var result = await _service.GetAllCampus();

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// Get a detail campus by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("getDetail/{Id}")]
        public async Task<IActionResult> GetDetailCampus(Guid Id)
        {
            var result = await _service.GetCampusById(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Update campus info
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("update/{Id}")]
        public async Task<IActionResult> Update(Guid Id, UpdateCampusModel model)
        {
            var result = await _service.UpdateCampus(Id, model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("disable/{Id}")]
        public async Task<IActionResult> Disable(Guid Id)
        {
            var result = await _service.DisableCampus(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

    }
}

