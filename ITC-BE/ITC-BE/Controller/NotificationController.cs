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
    [Route("api/v1/notification")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _service;
        public NotificationController(INotificationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get a noti  by AccountId
        /// </summary>
        /// <param name="AccountId"></param>
        /// <returns></returns>
        [HttpGet("getNotificationByAccountId/{accountId}")]
        public async Task<IActionResult> GetNotificationByAccountId(Guid accountId)
        {
            var result = await _service.GetNotificationByAccountId(accountId);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Update Rea
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("updateStatusNotificationByNotiId/{Id}")]
        public async Task<IActionResult> UpdateStatusNotificationByNotiId(Guid Id)
        {
            var result = await _service.UpdateStatusNotificationByNotiId(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

    }
}

