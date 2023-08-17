using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITC.Core.Interface;
using ITC.Core.Model;
using ITC.Data.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ITC_BE.Controller
{
    [Route("api/v1/partner")]
    public class PartnerController : ControllerBase
    {
        private readonly IPartnerService _service;
        public PartnerController(IPartnerService service)
        {
            _service = service;
        }

        /// <summary>
        /// Create a new partner
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateNewPartNer(PartnerCreateModel model)
        {
            var result = await _service.CreatePartner(model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// Get all Partner
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAllPartner")]
        public async Task<IActionResult> GetAllPartner()
        {
            var result = await _service.GetPartner();

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// Get a detail parner by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("getDetail/{Id}")]
        public async Task<IActionResult> GetPartnerById(Guid Id)
        {
            var result = await _service.GetDetailPartner(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

         /// <summary>
        /// search Partner
        /// </summary>
        /// <returns></returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchPartnerName(string keyword)
        {
            var result = await _service.SearchPartner(keyword);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// disable parner by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost("disable/{Id}")]
        public async Task<IActionResult> DisablePartner(Guid Id)
        {
            var result = await _service.DisablePartner(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// update a detail Partner by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("update/{Id}")]
        public async Task<IActionResult> Update(Guid Id, PartnerUpdateModel model)
        {
            var result = await _service.UpdatePartner(Id, model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


    }
}

