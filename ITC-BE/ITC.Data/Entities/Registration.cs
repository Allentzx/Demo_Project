using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITC.Data.Entities
{
    public class Registration
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? ParentRegistrationsId { get; set; }
        public string? Title { get; set; }
        public string? NumberPassPort { get; set; }
        public string? PassportImageUrl { get; set; }
        public string? UrlImageBill { get; set; }
        public string? ScocialLink { get; set; }
        public string? YourEmail { get; set; }
        public string? Creator { get; set; }
        public string? Comment { get; set; }
        public bool Status { get; set; }
        public DateTime? DateExpired { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateOpenRegis { get; set; }
        public DateTime? DateCloseRegis { get; set; }


        public Registration? ParentRegistrations { get; set; }
        public ICollection<Registration>? ChildrenRegistrations { get; set; }
        public ICollection<RegistrationAddOn>? RegistrationAddOn { get; set; }
        public ICollection<FeedBack>? FeedBacks { get; set; }

        // FK 
        public Guid? StudentId { get; set; }
        public Guid? ProjectId { get; set; }

        public virtual Student? Student { get; set; }
        public virtual Project? Project { get; set; }



    }
}
