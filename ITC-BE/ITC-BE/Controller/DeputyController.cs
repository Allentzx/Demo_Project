using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITC.Core.Interface;
using ITC.Core.Model;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ITC_BE.Controller
{
    [Route("api/v1/deputy")]
    public class DeputyController : ControllerBase
    {
        private readonly IDeputyService _service;

        public DeputyController(IDeputyService service)
        {
            _service = service;
        }

        /// <summary>
        /// Create a new Deputy
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateNewDeputy(CreateDeputyDTO model)
        {
            var result = await _service.CreateDeputy(model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Get all Deputy
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllDeputy()
        {
            var result = await _service.GetAllDeputy();

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Update Deputy by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("update/{Id}")]
        public async Task<IActionResult> UpdateDeputy(Guid Id, UpdateDeputyDTO model)
        {
            var result = await _service.UpdateDeputy(Id, model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }
    }
}

