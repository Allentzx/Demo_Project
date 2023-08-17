using System;
namespace ITC.Data.Entities
{
	public class FeedBack
	{
        public Guid Id { get; set; }
        public Guid? ParenFeedBacksId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? FeedBackContent { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Status { get; set; }

        public FeedBack? FeedBacks{ get; set; }
        public ICollection<FeedBack>? ChildrenFeedBacKs { get; set; }
        public ICollection<FeedBackAddOn>? FeedBackAddOns { get; set; }

        public Guid? RegistrationId { get; set; }
        public virtual Registration? Registration { get; set; }

    }
}

