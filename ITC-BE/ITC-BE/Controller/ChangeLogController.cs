using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITC.Core.Interface;
using ITC.Core.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ITC_BE.Controller
{
    [Route("api/v1/changelog")]
    public class ChangeLogController : ControllerBase
    {
        private readonly IAuditService _service;
        public ChangeLogController(IAuditService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all Changelog History
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAllChangelog")]
        public async Task<IActionResult> getAllChangelog()
        {
            var result = await _service.GetAllChangeLog();

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Get a detail changelog detail by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("getDetail/{Id}")]
        public async Task<IActionResult> GetDetailChangeLog(int Id)
        {
            var result = await _service.GetDetailChangeLog(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Get a GetChangeLogTasks
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("getChangeLogTasks")]
        public async Task<IActionResult> GetChangeLogTasks()
        {
            var result = await _service.GetChangeLogTasks();

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }
    }
}

