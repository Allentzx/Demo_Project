using System;
using System.ComponentModel.DataAnnotations;

namespace ITC.Data.Entities
{
	public class Document
	{
        [Key]
        public Guid Id { get; set; }
        public string? FileName { get; set; }
        public string? FileExtenstion { get; set; }
        public string? Owner { get; set; }
        public string? MarkReportUrl { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Status { get; set; }

        //Fk
        public Guid ProjectId { get; set; }
        public virtual Project? Project { get; set; }
    }
}

