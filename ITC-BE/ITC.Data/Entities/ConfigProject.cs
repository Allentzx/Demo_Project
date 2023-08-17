using System;
using System.ComponentModel.DataAnnotations;

namespace ITC.Data.Entities
{
	public class ConfigProject
	{
        [Key]
        public Guid Id { get; set; }
        public string? MinStudent { get; set; }
        public string? MaxStudent { get; set; }
        public string? MinStaff { get; set; }
        public string? MaxStaff { get; set; }
        public string? TermTime { get; set; }
        public string? TotalProjectLeader { get; set; }
        public DateTime? DateCreated  { get; set; }
    }
}

