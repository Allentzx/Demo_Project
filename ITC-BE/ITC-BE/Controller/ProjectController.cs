using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITC.Data.Enum;
using ITC.Core.Interface;
using ITC.Core.Model;
using ITC.Data.Utilities.Paging.PaginationModel;
using Microsoft.AspNetCore.Mvc;


namespace ITC_BE.Controller
{
    [Route("api/v1/project")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _service;
        public ProjectController(IProjectService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateProject([FromForm] ProjectCreateModel model)
        {
            var result = await _service.CreateProject(model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// Get all project
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAllProject")]
        public async Task<IActionResult> GetAllProject()
        {
            var result = await _service.GetAllProject();

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Get all JoinProject                
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAllJoinProject")]
        public async Task<IActionResult> GetAllJoinProject()
        {
            var result = await _service.GetJoinProject();

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Get a join project by projectId 
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet("getJoin/{projectId}")]
        public async Task<IActionResult> getJoin(Guid projectId)
        {
            var result = await _service.GetJoinProjectId(projectId);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Delete project by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("disable/{Id}")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var result = await _service.DeleteProject(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Update project
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("update/{Id}")]
        public async Task<IActionResult> UpdateProject(Guid Id, [FromQuery] ProjectUpdateModel model)
        {
            var result = await _service.UpdateProject(Id, model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// Get a detail project by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("getDetail/{Id}")]
        public async Task<IActionResult> GetProjectById(Guid Id)
        {
            var result = await _service.GetDetailProject(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        [HttpPost("assign/{Id}")]
        public async Task<IActionResult> AssignStaff(Guid Id, [FromForm] List<Guid>? staffId)
        {
            var result = await _service.AssignStaffIntoProject(Id, staffId);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("unassign/{projectId}")]
        public async Task<IActionResult> UnAssignStaff(Guid projectId, Guid staffId)
        {
            var result = await _service.UnAssignStaffIntoProject(projectId, staffId);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// search  Project
        /// </summary>
        /// <returns></returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchProjectName(string keyword)
        {
            var result = await _service.SearchProject(keyword);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// change status project
        /// </summary>
        /// <returns></returns>
        [HttpPost("changeStatus")]
        public async Task<IActionResult> ChangeStatus([FromForm]ChangeStatusProject model)
        {
            var result = await _service.ChangeStatusProject(model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

    }
}

