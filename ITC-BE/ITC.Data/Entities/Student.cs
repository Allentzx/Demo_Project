using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Data.Entities
{
    public class Student
    {
        [Key]
        public Guid Id { get; set; }
        public string? RollNumber { get; set; }
        public string? MemberCode { get; set; }
        public string? FullName { get; set; }
        public string? OldRollNumber { get; set; }
        public string? MajorName { get; set; }
        public string? Batch { get; set; }
        public string? Semeter { get; set; }
        public string? StudentStatus { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public bool Status { get; set; }
        public string? GradingUrl { get; set; }
        public Guid? MajorId { get; set; }

        public virtual Major? Major { get; set; }
        public ICollection<Registration>? Registration { get; set; }

        // public ICollection<Notification>? Notifications { get; set; }
    }
}
