using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITC.Core.Interface;
using ITC.Core.Model;
using Microsoft.AspNetCore.Mvc;

namespace ITC_BE.Controller
{
    [Route("api/v1/slot")]
    public class SlotController : ControllerBase
    {
        private readonly ISlotService _service;
        public SlotController(ISlotService service)
        {
            _service = service;
        }

        /// <summary>
        /// Create a new slot
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateNewSlot(CreateSlotModel model)
        {
            var result = await _service.CreateSlot(model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// Get all Slot
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAllSlot")]
        public async Task<IActionResult> GetAllSlot()
        {
            var result = await _service.GetAllSlot();

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// Get a detail slot by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("getDetail/{Id}")]
        public async Task<IActionResult> GetSlotById(int Id)
        {
            var result = await _service.GetDetailSlot(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Disable Slot by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("disable/{Id}")]
        public async Task<IActionResult> DisableSlot(int Id)
        {
            var result = await _service.DeleteSlot(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Update Slot detail
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("update/{Id}")]
        public async Task<IActionResult> Update(int Id, UpdateSlotModel model)
        {
            var result = await _service.UpdateSlot(Id, model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Update Slot Status
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("updateStatus/{Id}")]
        public async Task<IActionResult> UpdateStatus(int Id, UpdateSlotStatus model)
        {
            var result = await _service.UpdateSlotStatus(Id, model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }
    }
}

