using Azure.Storage.Blobs;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Google.Apis.Auth;
using ITC.Core.Configurations;
using ITC.Core.Data;
using ITC.Core.Interface;
using ITC.Core.Model;
using ITC.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace ITC_BE.Controller
{
    [Route("api/v1/student")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _service;

        private readonly ITCDBContext _context;
        private readonly IAzureBlobStorageService _storage;
        private readonly string _storageConnectionString;
        private readonly string _storageContainerName;
        private readonly GoogleAuthConfiguration _googleAuthConfiguration;
        public StudentController(IStudentService service, IOptions<GoogleAuthConfiguration> options
                                    , IConfiguration configuration, ITCDBContext context,IAzureBlobStorageService storage)
        {
            _service = service;
            _googleAuthConfiguration = options.Value;
            _context = context;
            _storageConnectionString = configuration["BlobConnectionString"];
            _storageContainerName = configuration["BlobContainerName"];
            _storage = storage;
        }


        /// <summary>
        /// student login with goggle
        /// </summary>
        /// <returns></returns>
        [HttpPost("signin-google/{token}")]
        public async Task<IActionResult> GoogleAuthenticateStudent([FromRoute] string token)
        {
            var googleUser = await GoogleJsonWebSignature.ValidateAsync(token,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _googleAuthConfiguration.ClientId }
                });

            var result = await _service.GoogleAuthenticateStudent(googleUser);

            if (result is { IsSuccess: true, Code: 200 }) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("importStudent")]
        public async Task<IActionResult> ImportStudent([FromForm]ImportStudentModel model)
        {
            var result = await _service.ImportStudent(model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// Get All Student
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAllStudent")]
        public async Task<IActionResult> GetAllStudent()
        {
            var result = await _service.GetStudent();

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// get a student by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("getStudentDetail/{Id}")]
        public async Task<IActionResult> GetDetailStudent(Guid Id)
        {
            var result = await _service.GetDetailStudent(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// Delete student by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{Id}")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var result = await _service.DeleteStudent(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Update Student
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("update/{Id}")]
        public async Task<IActionResult> UpdateStudent(Guid Id, UpdateStudentModel model)
        {
            var result = await _service.UpdateStudent(Id, model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// Create a new student
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateNewStudent([FromForm]CreateStudentModel model)
        {
            var result = await _service.CreateStudent(model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// export list student
        /// </summary>
        /// <returns></returns>
        [HttpGet("exportExcel")]
        public async Task<IActionResult> ExportToExcel()
        {
            var result = await _service.ExportStudentToExcel();
            // return Ok(result);
            return File(result.Data, result.FileType, result.FileName);
        }


        /// get a content gradingUrl by studentId
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("content/{studentId}")]
        public async Task<IActionResult> GetContent(Guid studentId)
        {
            var result = await _service.GetContent(studentId);
            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        // <summary>
        /// Upload Image Grading new Cancel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("upload/{studentId}")]
        public async Task<IActionResult> Upload(Guid studentId, IFormFile? FormFile)
        {
            var result = await _service.UploadGrading(studentId, FormFile);
            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// GetGradingStudentId
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetGradingStudentId/{studentId}")]
        public async Task<IActionResult> GetGradingStudentId(Guid studentId)
        {
            var result = await _service.GetGradingStudentId(studentId);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// DeleteGrading student by id
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        [HttpDelete("deleteGrading/{studentId}")]
        public async Task<IActionResult> DeleteGrading(Guid studentId)
        {
            var result = await _service.DeleteGrading(studentId);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }
    }
}
