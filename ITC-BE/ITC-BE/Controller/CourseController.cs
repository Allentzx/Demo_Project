using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITC.Core.Interface;
using ITC.Core.Model;
using Microsoft.AspNetCore.Mvc;

namespace ITC_BE.Controller
{
    [Route("api/v1/course")]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _service;
        public CourseController(ICourseService service)
        {
            _service = service;
        }

        /// <summary>
        /// Create a new Course
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateNewCourse(CreateCourseModel model)
        {
            var result = await _service.CreateCourse(model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// Get all Course
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAllCourse")]
        public async Task<IActionResult> GetAllCourse()
        {
            var result = await _service.GetAllCourse();

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Get a detail course by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("getDetail/{Id}")]
        public async Task<IActionResult> GetCourseById(Guid Id)
        {
            var result = await _service.GetDetailCourse(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Delete Course by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("delete/{Id}")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var result = await _service.DeleteCourse(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Update course
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("update/{Id}")]
        public async Task<IActionResult> Update(Guid Id, UpdateCourseModel model)
        {
            var result = await _service.UpdateCourse(Id, model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

    }
}

