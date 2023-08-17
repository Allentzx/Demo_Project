using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ITC.Core.Model
{
    public class PostModel
    {
        public Guid Id { get; set; }
        public string? Author { get; set; }
        public string? Title { get; set; }
        public string? SubTitle { get; set; }
        public string? PosterUrl { get; set; }
        [MaxLength(1000)]
        public string? Content { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Status { get; set; }

        //Fk
        public Guid StaffId { get; set; }
    }
    public class PostViewModel
    {
        public Guid Id { get; set; }
        public string? Author { get; set; }
        public string? Title { get; set; }
        public string? SubTitle { get; set; }
        public string? PosterUrl { get; set; }
        public DateTime PostDate { get; set; }
        public bool Status { get; set; }
        public Guid StaffId { get; set; }

    }

    

    public class PostResponse
    {
        public string? Status { get; set; }
        public bool Error { get; set; }
        public string? Uri { get; set; }
        public string? Name { get; set; }
        public string? ContentType { get; set; }
        public Stream? Content { get; set; }
        public CreatePostUploadApiRequest PostRequest { get; set; }

        public PostResponse()
        {
            PostRequest = new CreatePostUploadApiRequest();
        }
    }

    public class CreatePostUploadApiRequest
    {
        public IFormFile? FormFile { get; set; }
        public string? Title { get; set; }
        public string? SubTitle { get; set; }
        public string? Author { get; set; }
        public string? Content { get; set; }
    }



    public class UpdatePostResponse
    {
        public string? Status { get; set; }
        public bool Error { get; set; }
        public string? Uri { get; set; }
        public string? Name { get; set; }
        public string? ContentType { get; set; }
        public Stream? Content { get; set; }
        public UpdateFilePostResponse UpdatePostRequest { get; set; }

        public UpdatePostResponse()
        {
            UpdatePostRequest = new UpdateFilePostResponse();
        }
    }

    public class UpdateFilePostResponse
    {
        public IFormFile? FormFile { get; set; }
        public string? Author { get; set; }
        public string? Title { get; set; }
        public string? SubTitle { get; set; }
        public string? Content { get; set; }
        public bool Status { get; set; }

    }

}
