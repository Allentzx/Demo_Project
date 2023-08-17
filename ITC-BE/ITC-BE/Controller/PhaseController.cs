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
    [Route("api/v1/phase")]
    [ApiController]
    public class PhaseController : ControllerBase
    {

        private readonly IPhaseService _service;

        public PhaseController(IPhaseService service)
        {
            _service = service;
        }

        /// <summary>
        /// Create a new Phase
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateNewPhase([FromForm] PhaseCreateModel model)
        {
            var result = await _service.CreatePhase(model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Get all Phase
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAllPhase")]
        public async Task<IActionResult> getAllPhase()
        {
            var result = await _service.GetAllPhase();

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// Get all Phase
        /// </summary>
        /// <returns></returns>
        [HttpGet("getPhaseByProjectId/{projectId}")]
        public async Task<IActionResult> GetPhaseByProjectId(Guid? projectId)
        {
            var result = await _service.GetPhaseByProjectId(projectId);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// Delete Phase by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost("delete/{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            var result = await _service.DeletePhase(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Update Phase
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("update/{Id}")]
        public async Task<IActionResult> UpdatePhase(int Id, PhaseUpdateModel model)
        {
            var result = await _service.UpdateLPhase(Id, model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// changePhaseDate
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("changePhaseDate")]
        public async Task<IActionResult> ChangePhaseDate([FromForm] UpdateDatePhase model)
        {
            var result = await _service.UpdateDatePhase(model);
            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("addPhase/{projectId}")]
        public async Task<IActionResult> AddPhase(Guid projectId, [FromForm]AssignPhase model)
        {
            var result = await _service.AddPhaseIntoProject(projectId, model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("removePhase/{projectId}")]
        public async Task<IActionResult> RemovePhaseInProject(Guid projectId, int phaseId)
        {
            var result = await _service.RemovePhaseInProject(projectId, phaseId);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Update Phase Status
        /// </summary>
        /// <returns></returns>
        [HttpPut("updateStatusPhase")]
        public async Task<IActionResult> UpdatePhase(PhaseUpdateStatusModel model)
        {
            var result = await _service.UpdateStatusPhase(model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }
    }
}

