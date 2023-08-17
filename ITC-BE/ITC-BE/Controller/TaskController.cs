using ITC.Core.Interface;
using ITC.Core.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITC_BE.Controller
{
    [Route("api/v1/task")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _service;
        public TaskController(ITaskService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTask([FromQuery] TaskCreateModel model)
        {
            var result = await _service.CreateTask(model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }



        /// <summary>
        /// Assign emp
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost("assignTask/{TaskId}")]
        public async Task<IActionResult> AssignTask(Guid TaskId, Guid StaffId)
        {
            var result = await _service.AssignTask(TaskId, StaffId);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// Get Root task
        /// </summary>
        /// <returns></returns>
        [HttpGet("getRootsTask")]
        public async Task<IActionResult> GetRootTask()
        {
            var result = await _service.GetRootTask();

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Get all task
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAllTask")]
        public async Task<IActionResult> GetAllTask()
        {
            var result = await _service.GetAllTask();

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Get Child Task
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetChildTask/{parentId}")]
        public async Task<IActionResult> GetChildTask(Guid parentId)
        {
            var result = await _service.GetChildTask(parentId);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// disable task by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("DisableTask/{Id}")]
        public async Task<IActionResult> DisableTask(Guid Id)
        {
            var result = await _service.DisableTask(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Update task
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("update/{Id}")]
        public async Task<IActionResult> UpdateTask(Guid Id, [FromForm] TaskUpdateModel model)
        {
            var result = await _service.UpdateTask(Id, model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// Get a detail task by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("getTaskDetaul/{Id}")]
        public async Task<IActionResult> GetTaskById(Guid Id)
        {
            var result = await _service.GetDetailTask(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// unassign staff in  task by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost("unassign/{Id}")]
        public async Task<IActionResult> Unassign(Guid Id, Guid staffId)
        {
            var result = await _service.UnAssignTask(Id, staffId);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// change status task
        /// </summary>
        /// <returns></returns>
        [HttpPost("changeStatus/")]
        public async Task<IActionResult> ChangeStatus([FromForm]ChangeStatus model)
        {
            var result = await _service.ChangeStatusTask(model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// change state task
        /// </summary>
        /// <returns></returns>
        [HttpPost("changeStates")]
        public async Task<IActionResult> ChangeState([FromForm]ChangeState model)
        {
            var result = await _service.ChangeStateTask(model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// search task
        /// </summary>
        /// <returns></returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchTask(string keyword)
        {
            var result = await _service.SearchTask(keyword);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Get a detail task by projectId
        /// </summary>getTaskByProjectId
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet("getTaskByProjectId/{projectId}")]
        public async Task<IActionResult> GetTaskByProjectId(Guid projectId)
        {
            var result = await _service.GetTaskByProjectId(projectId);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

    }
}
