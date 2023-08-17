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
    [Route("api/v1/cancel")]
    [ApiController]
    public class CancelController : ControllerBase
    {

        private readonly ICancelService _service;

        public CancelController(ICancelService service)
        {
            _service = service;
        }


        /// <summary>
        /// Create new Cancel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateCancel([FromForm] CancelUploadApiRequest model)
        {
            var result = await _service.CreateAsync(model);
            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// get a Document detail by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("content/{Id}")]
        public async Task<IActionResult> GetContent(Guid Id)
        {
            CancelResponse? file = await _service.GetContent(Id);

            // Check if file was found
            if (file == null)
            {
                // Was not, return error message to client
                return StatusCode(StatusCodes.Status500InternalServerError, $"File {Id} could not be downloaded.");
            }
            else
            {
                // File was found, return it to client
                return File(file.Content, file.ContentType, file.Name);
            }
        }

        /// <summary>
        /// get a cancel by projectId
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet("getProjectCancel/{projectId}")]
        public async Task<IActionResult> GetProjectCancel(Guid projectId)
        {
            var result = await _service.GetProjectCancel(projectId);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }



     
    }

}

