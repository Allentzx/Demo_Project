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
    [Route("api/v1/syllabus")]
    public class SyllabusController : ControllerBase
    {
        private readonly ISyllabusService _service;
        public SyllabusController(ISyllabusService service)
        {
            _service = service;
        }

        /// <summary>
        /// Create a new syllabus
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateNewCourse(CreateSyllabusModel model)
        {
            var result = await _service.CreateSyllabus(model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// Get all syllabus
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAllSyllabus")]
        public async Task<IActionResult> GetAllSyllabus()
        {
            var result = await _service.GetAllSyllabus();

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Get a detail syllabus by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("getDetail/{Id}")]
        public async Task<IActionResult> GetSyllabusById(Guid Id)
        {
            var result = await _service.GetDetailSyllabus(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Get a GetListSyllabusPartner
        /// </summary>
        /// <param name="partnerId"></param>
        /// <returns></returns>
        [HttpGet("GetListSyllabusPartner/{partnerId}")]
        public async Task<IActionResult> GetListSyllabusPartner(Guid? partnerId)
        {
            var result = await _service.GetListSyllabusPartner(partnerId);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Delete Syllabus by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{Id}")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var result = await _service.DeleteSyllabus(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Update Syllabus
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("update/{Id}")]
        public async Task<IActionResult> Update(Guid Id, UpdateSyllabusModel model)
        {
            var result = await _service.UpdateSyllabus(Id, model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Update Active Status Syllabus
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("changeStatusSyllabus/{Id}")]
        public async Task<IActionResult> ChangeStatusSyllabus(Guid Id, UpdateSyllabusStatusModel model)
        {
            var result = await _service.UpdateSyllabusStatus(Id, model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// search Syllabus
        /// </summary>
        /// <returns></returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchSyllabus(string keyword)
        {
            var result = await _service.SearchSyllabus(keyword);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }
    }
}

