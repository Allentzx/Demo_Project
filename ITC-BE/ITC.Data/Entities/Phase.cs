using System;
using System.ComponentModel.DataAnnotations;

namespace ITC.Data.Entities
{
	public class Phase
	{
		[Key]
		public int? Id { get; set; }
		public string? PhaseName { get; set; }
        public bool Status { get; set; }
		public DateTime DateCreate { get; set; }

		public virtual ICollection<ProjectPhase>? ProjectPhases { get; set; }


	}
}

