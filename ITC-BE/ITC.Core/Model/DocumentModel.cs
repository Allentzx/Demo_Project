using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Core.Model
{
    public class DocumentModel
    {

    }

    public class DocumentResponse
    {
        public string? Status { get; set; }
        public bool Error { get; set; }
        public string? Uri { get; set; }
        public string? Name { get; set; }
        public string? ContentType { get; set; }
        public Stream? Content { get; set; }
        public DocumentUploadApiRequest DocumentRequest { get; set; }

        public DocumentResponse()
        {
           DocumentRequest = new DocumentUploadApiRequest();
        }
    }

    public class DocumentUploadApiRequest
    {
        [Required]
        public Guid ProjectId { get; set; }
        public IFormFile? FormFile { get; set; }
        public string? FileName { get; set; }
        public string? FileExtenstion { get; set; }
        public string? Owner { get; set; }
        public string? MarkReportUrl { get; set; }
        public bool Status { get; set; }
    }

    public class DocumentUploadApiReponse
    {
        [Required]
        public Guid ProjectId { get; set; }
        public IFormFile? FormFile { get; set; }
        public string? FileName { get; set; }
        public string? FileExtenstion { get; set; }
        public string? Owner { get; set; }
        public string? MarkReportUrl { get; set; }
        public bool Status { get; set; }
    }

}
