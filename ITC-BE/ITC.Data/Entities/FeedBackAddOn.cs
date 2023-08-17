using System;
using System.ComponentModel.DataAnnotations;

namespace ITC.Data.Entities
{
	public class FeedBackAddOn
	{
        [Key]
        public Guid Id { get; set; }
        public Guid? FeedBackId { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
        public virtual FeedBack? FeedBack { get; set; }
    }
}

