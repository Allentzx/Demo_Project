using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Core.Model
{
    public class CommentModel
    {
    }


    public class ViewCommentInTask
    {
        public Guid Id { get; set; }
        public string? Comment { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
        public DateTime? DateCreated { get; set; }
        public Guid? StaffId { get; set; }
        public Guid TasksId { get; set; }

    }

    public class UpdateCommentResponse
    {
        public string? Status { get; set; }
        public bool Error { get; set; }
        public string? Uri { get; set; }
        public string? Name { get; set; }
        public string? ContentType { get; set; }
        public Stream? Content { get; set; }
        public UpdateFileCommentResponse UpdateCommentRequest { get; set; }

        public UpdateCommentResponse()
        {
            UpdateCommentRequest = new UpdateFileCommentResponse();
        }
    }

    public class UpdateFileCommentResponse
    {
        public IFormFile? FormFile { get; set; }
        public string? Comment { get; set; }
        public Guid TaskId { get; set; }
    }


    public class CommentResponse
    {
        public string? Status { get; set; }
        public bool Error { get; set; }
        public string? Uri { get; set; }
        public string? Name { get; set; }
        public string? ContentType { get; set; }
        public Stream? Content { get; set; }
        public CommentUploadApiRequest CommentRequest { get; set; }

        public CommentResponse()
        {
            CommentRequest = new CommentUploadApiRequest();
        }
    }

    public class CommentUploadApiRequest
    {
        public IFormFile? FormFile { get; set; }
        public string? Comment { get; set; }
        public Guid TaskId { get; set; }
    }

    public class CommentViewModel
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? Comment { get; set; }
        public string? FileUrl { get; set; }
        public DateTime? DateCreated { get; set; }
        public bool CheckEdit { get; set; }
        public Guid? StaffId { get; set; }
        public Guid TasksId { get; set; }
        public ICollection<ViewFileTracking>? ViewFile { get; set; }

        public class ViewFileTracking
        {
            public string? FileName { get; set; }
            public string? FileExtension { get; set; }
            public string? FileUrl { get; set; }
            public string? Owner { get; set; }
            public DateTime DateUpLoad { get; set; }
        }
    }
}
