using System;
using ITC.Data.Enum;
using Microsoft.AspNetCore.Http;

namespace ITC.Core.Model
{
	public class CancelModel
	{
		
	}

    public class CreateCancelModel
    {
        public IFormFile? FormFile { get; set; }
        public string? Description { get; set; }
        public Guid ProjectId { get; set; }
    }

    public class CancelResponse
    {
        public string? Status { get; set; }
        public bool Error { get; set; }
        public string? Uri { get; set; }
        public string? Name { get; set; }
        public string? ContentType { get; set; }
        public Stream? Content { get; set; }
        public CancelUploadApiRequest CancelRequest { get; set; }

        public CancelResponse()
        {
            CancelRequest = new CancelUploadApiRequest();
        }
    }

    public class CancelUploadApiRequest
    {
        public IFormFile? FormFile { get; set; }
        public string? Description { get; set; }
        public Guid? ProjectId { get; set; }
    }
}

