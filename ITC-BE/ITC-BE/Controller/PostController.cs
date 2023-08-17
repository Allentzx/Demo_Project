using DocumentFormat.OpenXml.VariantTypes;
using ITC.Core.Interface;
using ITC.Core.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITC_BE.Controller
{
    [Route("api/v1/post")]
    [ApiController]
    public class PostController : ControllerBase
    {

        private readonly IPostService _service;

        public PostController(IPostService service)
        {
            _service = service;
        }

        /// <summary>
        /// Create a new Post
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateNewPost([FromForm]CreatePostUploadApiRequest model)
        {
            var result = await _service.CreatePost(model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Get all Post
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAllPost")]
        public async Task<IActionResult> GetAllPost()
        {
            var result = await _service.GetAllPost();

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// get Post by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("getPostById/{Id}")]
        public async Task<IActionResult> GetPostById(Guid Id)
        {
            var result = await _service.GetPostById(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Delete Post by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{Id}")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var result = await _service.DeletePost(Id);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }


        /// <summary>
        /// Delete  File Image by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("deleteImage/{imageId}")]
        public async Task<IActionResult> DeleteImage(Guid imageId)
        {
            var result = await _service.DeleteImage(imageId);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Update Post
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPut("update/{Id}")]
        public async Task<IActionResult> UpdatePost(Guid Id,[FromForm] UpdateFilePostResponse model)
        {
            var result = await _service.UpdatePost(Id, model);

            if (result.IsSuccess && result.Code == 200) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("upload/{postId}")]
        public async Task<IActionResult> UploadList(Guid postId, List<IFormFile> file)
        {
            BlobResponseDto? response = await _service.UploadListImage(postId , file);

            // Check if we got an error
            if (response.Error == true)
            {
                // We got an error during upload, return an error with details to the client
                return StatusCode(StatusCodes.Status500InternalServerError, response.Status);
            }
            else
            {
                // Return a success message to the client about successfull upload
                return StatusCode(StatusCodes.Status200OK, response);
            }
        }
    }
}
