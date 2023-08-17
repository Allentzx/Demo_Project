using System;
using System.ComponentModel.DataAnnotations;

namespace ITC.Core.Model
{
	public class PartnerModel
	{
        public string? Name { get; set; }
        public string? Local { get; set; }
        public string? Note { get; set; }
        public bool Status { get; set; }
    }

    public class PartnerViewModel : PartnerModel
    {
        public Guid Id { get; set; }
    }

    public class PartnerCreateModel
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Local { get; set; }
        public string? Note { get; set; }
        public bool Status { get; set; }
    }


    public class PartnerUpdateModel
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Local { get; set; }
        public string? Note { get; set; }
        public bool Status { get; set; }
    }
}

