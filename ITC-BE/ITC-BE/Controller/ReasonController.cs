using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITC.Core.Interface;
using ITC.Core.Model;
using Microsoft.AspNetCore.Mvc;


namespace ITC_BE.Controller
{
    [Route("api/v1/reason")]
    [ApiController]
    public class ReasonController : ControllerBase
    {

        private readonly IReasonService _service;

        public ReasonController(IReasonService service)
        {
            _service = service;
        }

        /// <summary>
        /// Create a reason
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateNewReason([FromForm] CreateReasonModel model)
        {
            var result = await _service.CreateReason(model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Get all reason
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAllReason")]
        public async Task<IActionResult> GetAllReason()
        {
            var result = await _service.GetAllReason();

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// get a reason by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("getReasonDetail/{Id}")]
        public async Task<IActionResult> getReasonDetail(Guid Id)
        {
            var result = await _service.GetDetailReason(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

    }
}

