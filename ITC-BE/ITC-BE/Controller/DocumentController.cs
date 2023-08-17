using ITC.Core.Interface;
using ITC.Core.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITC_BE.Controller
{
    [Route("api/v1/document")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _service;

        public DocumentController(IDocumentService service)
        {
            _service = service;
        }

        /// <summary>
        /// Create new Document by current user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateDoccument([FromForm] DocumentUploadApiRequest model)
        {
            var result = await _service.CreateDocument(model);
            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// get all Document 
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAll")]
        public async Task<IActionResult> GetDocument()
        {
            var result = await _service.GetAllDocument();
            if (result != null)
            {
                return Ok(result);
            }
            return NotFound();
        }

       
        /// <summary>
        /// get a Document detail by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetDocumentDetail(Guid Id)
        {
            var result = await _service.GetDetailDocument(Id);
            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// get a Document detail by projectId
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetDetailDocumentByProjectId(Guid projectId)
        {
            var result = await _service.GetDetailDocumentByProjectId(projectId);
            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// get a Document detail by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("content/{Id}")]
        public async Task<IActionResult> GetContent(Guid Id)
        {
            DocumentResponse? file = await _service.GetContent(Id);

            // Check if file was found
            if (file == null)
            {
                // Was not, return error message to client
                return StatusCode(StatusCodes.Status500InternalServerError, $"File {Id} could not be downloaded.");
            }
            else
            {
                // File was found, return it to client
                return File(file.Content, file.ContentType, file.Name);
            }
        }
    }
}
