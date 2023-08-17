using System;
using System.ComponentModel.DataAnnotations;

namespace ITC.Data.Entities
{
	public class Reason
	{
        [Key]
		public Guid? Id { get; set; }
        public string? ReasonContent { get; set; }
        public DateTime DateCreated { get; set; }
      

        //FK
        public Guid? DeputyId { get; set; }
        public int? SlotId { get; set; }
        
        public Deputy? Deputies { get; set; }
        public Slot? Slot { get; set; }
    }
}

