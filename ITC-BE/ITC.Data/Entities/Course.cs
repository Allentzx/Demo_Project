using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ITC.Data.Entities
{
    public class Course
    {
        [Key]
        public Guid Id { get; set; }
        [StringLength(50)]
        public string? CourseName { get; set; }
        [MaxLength(100)]
        public string? Activity { get; set; }
        [MaxLength(1000)]
        public string? Content { get; set; }
        public DateTime? DateCreated { get; set; }
        public Account? Creator { get; set; }
        public bool Status { get; set; }

        public ICollection<Syllabus>? Syllabus { get; set; }
        public ICollection<Project>? Projects { get; set; }

    }
}
