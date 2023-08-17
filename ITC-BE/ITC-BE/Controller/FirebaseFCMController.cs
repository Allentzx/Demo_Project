using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.Excel;
using ITC.Core.Interface;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ITC_BE.Controller
{
    [Route("api/v1/firebasefcm")]
    public class FirebaseFCMController : ControllerBase
    {
        private readonly IFirebaseFCMService _serFB;

        public FirebaseFCMController(IFirebaseFCMService serFB)
        {
            _serFB = serFB;
        }
        /// <summary>
        /// Save firebase token of account when Login success.
        /// </summary>    
        [HttpPost]
        public async Task<IActionResult> SaveTokenDevice([FromForm] Guid accountId, [FromForm] string? token)
        {
            var result = await _serFB.SaveToken(accountId, token);
            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }
    }
}

