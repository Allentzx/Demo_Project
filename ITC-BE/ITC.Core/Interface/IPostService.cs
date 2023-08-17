using ITC.Core.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Core.Interface
{
    public interface IPostService
    {
        Task<ResultModel> CreatePost(CreatePostUploadApiRequest model);
        Task<ResultModel> DeletePost(Guid id);
        Task<ResultModel> GetAllPost();
        Task<ResultModel> UpdatePost(Guid Id, UpdateFilePostResponse model);
        Task<ResultModel> GetPostById(Guid Id);
        Task<BlobResponseDto> UploadListImage(Guid postId, List<IFormFile> file);
        Task<ResultModel> DeleteImage(Guid imageId);
    }
}
