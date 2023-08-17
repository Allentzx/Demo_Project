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
    [Route("api/v1/Major")]
    [ApiController]
    public class MajorController : ControllerBase
    {

        private readonly IMajorService _service;

        public MajorController(IMajorService service)
        {
            _service = service;
        }

        /// <summary>
        /// Create a new Major
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateNewMajor(CreateMajorModel model)
        {
            var result = await _service.CreateMajor(model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Get all Major
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAllMajor")]
        public async Task<IActionResult> getAllMajor()
        {
            var result = await _service.GetAllMajor();

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// Update Major
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("update/{Id}")]
        public async Task<IActionResult> UpdateMajor(Guid Id, UpdateMajorModel model)
        {
            var result = await _service.UpdateMajor(Id, model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// detail Major
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("detail/{Id}")]
        public async Task<IActionResult> Detail(Guid Id)
        {
            var result = await _service.GetDetailMajor(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// disable Major
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("disable/{Id}")]
        public async Task<IActionResult> Disable(Guid Id)
        {
            var result = await _service.DeleteMajor(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// search  Major
        /// </summary>
        /// <returns></returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchMajorName(string keyword)
        {
            var result = await _service.SearchMajor(keyword);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

    }
}

