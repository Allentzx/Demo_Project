using System;
using System.ComponentModel.DataAnnotations;

namespace ITC.Data.Entities
{
	public class RegistrationAddOn
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? RegistrationId { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }

        public virtual Registration? Registration { get; set; }


    }
}

