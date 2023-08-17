using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITC.Core.Interface;
using ITC.Core.Model;
using Microsoft.AspNetCore.Mvc;

namespace ITC_BE.Controller
{
    [Route("api/v1/program")]
    public class ProgramController : ControllerBase
    {
        private readonly IProgramService _service;
        public ProgramController(IProgramService service)
        {
            _service = service;
        }

        /// <summary>
        /// Create a new Program
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateNewProgram(CreateProgramModel model)
        {
            var result = await _service.CreateProgram(model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// Get all Program
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAllProgram")]
        public async Task<IActionResult> GetAllProgram()
        {
            var result = await _service.GetAllProgram();

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Get a detail Program by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("getDetail/{Id}")]
        public async Task<IActionResult> GetProgramById(Guid Id)
        {
            var result = await _service.GetDetailProgram(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// disable Program by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("disable/{Id}")]
        public async Task<IActionResult> Disable(Guid Id)
        {
            var result = await _service.DeleteProgram(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Update Program
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("update/{Id}")]
        public async Task<IActionResult> Update(Guid Id, UpdateProgramModel model)
        {
            var result = await _service.UpdateProgram(Id, model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

    }
}

