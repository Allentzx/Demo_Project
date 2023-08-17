using ITC.Core.Interface;
using ITC.Core.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITC_BE.Controller
{
    [Route("api/v1/configProject")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IConfigService _service;
        public ConfigController(IConfigService service)
        {
            _service = service;
        }

        [HttpPost("importConfig")]
        public async Task<IActionResult> ImportConfig(IFormFile formFile)
        {
            var result = await _service.ImportConfig(formFile);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }
        
    }
}
