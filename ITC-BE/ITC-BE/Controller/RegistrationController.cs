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
    [Route("api/v1/registration")]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationService _service;
        public RegistrationController(IRegistrationService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreateRegistrationModel model)
        {
            var result = await _service.CreateNew(model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("createOptional")]
        public async Task<IActionResult> CreateOptional([FromForm] CreateOptionalModel model)
        {
            var result = await _service.CreateNewOptional(model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }
        /// <summary>
        /// Get all regis
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAllRes")]
        public async Task<IActionResult> GetAllRes()
        {
            var result = await _service.GetAll();

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// get detail res at studentId
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("getDetailbyStudentId/{studentId}")]
        public async Task<IActionResult> GetDetailResByStudentId(Guid studentId)
        {
            var result = await _service.GetDetailResByStudentId(studentId);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// get detail res at projectId
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("getDetailbyProjectId/{projectId}")]
        public async Task<IActionResult> GetDetailResByProjectId(Guid projectId)
        {
            var result = await _service.GetDetailResByProjectId(projectId);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }
        /// <summary>
        /// get detail res at resId
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("GetDetailResId/{Id}")]
        public async Task<IActionResult> GetDetailResId(Guid Id)
        {
            var result = await _service.GetDetailResId(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// get detail res at parentId
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [HttpGet("GetChildReg/{parentId}")]
        public async Task<IActionResult> GetChildReg(Guid parentId)
        {
            var result = await _service.GetChildReg(parentId);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// update detail res at studentId
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("updateRegisByStudentId/{studentId}")]
        public async Task<IActionResult> UpdateRegisByStudentId(Guid studentId, UpdateRegisInfoModel model)
        {
            var result = await _service.UpdateRegisByStudentId(studentId, model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// update detail res at resId
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("UpdateRegisId/{Id}")]
        public async Task<IActionResult> UpdateRegisId(Guid Id, UpdateRegisModel model)
        {
            var result = await _service.UpdateRegisId(Id, model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// update status res at resId
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("UpdateRegisStatus/{Id}")]
        public async Task<IActionResult> UpdateRegisStatus(Guid Id, bool status)
        {
            var result = await _service.UpdateRegisStatus(Id, status);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }
        /// <summary>
        /// Get root regis
        /// </summary>
        /// <returns></returns>
        [HttpGet("getRootRegis")]
        public async Task<IActionResult> GetRootRegis()
        {
            var result = await _service.GetRootRegis();

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("deleteOptional/{resId}")]
        public async Task<IActionResult> DeleteOption(Guid resId, Guid optionId)
        {
            var result = await _service.DeleteOption(resId, optionId);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// UpdateQuestion
        /// </summary>
        /// <returns></returns>
        [HttpPut("updateQuestion")]
        public async Task<IActionResult> UpdateQuestion(UpdateQuestionModel model)
        {
            var result = await _service.UpdateQuestion( model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// UpdateAnswer
        /// </summary>
        /// <returns></returns>
        [HttpPut("updateAnswer")]
        public async Task<IActionResult> UpdateAnswer(UpdateAnswerModel model)
        {
            var result = await _service.UpdateAnswer( model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


    }
}

