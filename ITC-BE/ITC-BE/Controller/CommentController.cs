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
    [Route("api/v1/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {

        private readonly ICommentService _service;

        public CommentController(ICommentService service)
        {
            _service = service;
        }

        

        /// <summary>
        /// Create a new Comment In post
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("createCommentTask")]
        public async Task<IActionResult> CreateCommentTask([FromForm] CommentUploadApiRequest model)
        {
            var result = await _service.CreateTaskComment(model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Get GetCommentInTask
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCommentInTask")]
        public async Task<IActionResult> GetCommentInTask()
        {
            var result = await _service.GetTaskComment();

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }
        /// <summary>
        /// Get GetStaffComment
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCommentByTaskId/{taskId}")]
        public async Task<IActionResult> GetCommentByTaskId(Guid taskId)
        {
            var result = await _service.GetCommentByTaskId(taskId);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// Delete comment by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{Id}")]
        public async Task<IActionResult> DeleteTaskComment(Guid Id)
        {
            var result = await _service.DeleteTaskComment(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Delete FileComment by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("deleteFile/{Id}")]
        public async Task<IActionResult> DeleteFileAsync(Guid Id)
        {
            var result = await _service.DeleteFileAsync(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Update comment in task
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("updateTaskComment/{Id}")]
        public async Task<IActionResult> Update(Guid Id,[FromForm] CommentUploadApiRequest model)
        {
            var result = await _service.UpdateTaskComment(Id, model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }
    }
}
