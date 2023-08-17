using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITC.Core.Interface;
using ITC.Core.Model;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ITC_BE.Controller
{
    [Route("api/v1/feedback")]
    [ApiController]
    public class FeedBackController : ControllerBase
    {
        private readonly IFeedBackService _service;
        public FeedBackController(IFeedBackService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreateFeedbackModel model)
        {
            var result = await _service.CreateNew(model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("createQuestion")]
        public async Task<IActionResult> CreateFeedbacKQuestion([FromForm] CreateFeedbacKQuestionModel model)
        {
            var result = await _service.CreateFeedbacKQuestion(model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// get detail fb at studentId
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("getDetailFeedBackByStudentId/{studentId}")]
        public async Task<IActionResult> GetDetailFeedBackByStudentId(Guid studentId)
        {
            var result = await _service.GetDetailFeedBackByStudentId(studentId);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// get detail fb at registration
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("getDetailByRes/{resId}")]
        public async Task<IActionResult> GetDetailByRes(Guid resId)
        {
            var result = await _service.GetDetailByRes(resId);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }
        /// <summary>
        /// get detail fb at resId
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("getDetailFbId/{Id}")]
        public async Task<IActionResult> GetDetailFbId(Guid Id)
        {
            var result = await _service.GetDetailFbId(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// get detail fb at parentId
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [HttpGet("getChildFb/{parentId}")]
        public async Task<IActionResult> GetChildFb(Guid parentId)
        {
            var result = await _service.GetChildFb(parentId);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }



        /// <summary>
        /// update detail fb at fbId
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("UpdateFeedBackId/{Id}")]
        public async Task<IActionResult> UpdateFeedBackId(Guid Id, UpdateFeedbackInfoModel model)
        {
           var result = await _service.UpdateFeedBackId(Id, model);

           if (result.IsSuccess && result.Code == 200) return Ok(result);
           return BadRequest(result);
        }

        /// <summary>
        /// Get root fb
        /// </summary>
        /// <returns></returns>
        [HttpGet("getRootFeedBack")]
        public async Task<IActionResult> GetRootFeedBack()
        {
            var result = await _service.GetRootFeedBack();

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("deleteQuestion/{Id}")]
        public async Task<IActionResult> DeleteQuestion(Guid Id, Guid questionId)
        {
            var result = await _service.DeleteQuestion(Id, questionId);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// UpdateQuestion
        /// </summary>
        /// <returns></returns>
        [HttpPut("updateFbQuestion")]
        public async Task<IActionResult> UpdateFbQuestion(UpdateFeedBackQuestionModel model)
        {
            var result = await _service.UpdateQuestion(model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// UpdateAnswer
        /// </summary>
        /// <returns></returns>
        [HttpPut("updateFbAnswer")]
        public async Task<IActionResult> UpdateFbAnswer(UpdateFeedBackAnswerModel model)
        {
            var result = await _service.UpdateAnswer(model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

    }
}

