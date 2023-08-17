using System;
using System.ComponentModel.DataAnnotations;

namespace ITC.Data.Entities
{
	public class CancelProject
    {
		[Key]
        public Guid? Id { get; set; }
        public string? HeaderName { get; set; }
        public Guid? HeaderId { get; set; }
        public string? Description { get; set; }
        public DateTime DateCreated { get; set; }
        public string? PhaseName { get; set; }
        //FK
        public Guid? ProjectId { get; set; }
        public Project? Project { get; set; }

    }
}

