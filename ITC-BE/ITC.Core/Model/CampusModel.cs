using System;
using ITC.Data.Entities;

namespace ITC.Core.Model
{
	public class CampusModel
	{
	}

    public class CreateCampusModel
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public Guid PartnerId { get; set; }
    }

    public class UpdateCampusModel
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public bool Status { get; set; }
        public Guid PartnerId { get; set; }
    }

    public class CampusViewModel
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public bool Status { get; set; }
        public Guid PartnerId { get; set; }
        public virtual PartnerViewModel? Partner { get; set; }
    }
}

