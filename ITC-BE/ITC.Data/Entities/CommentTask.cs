using System.ComponentModel.DataAnnotations;

namespace ITC.Data.Entities
{
    public class CommentTask
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? StaffId { get; set; }
        public string? FullName { get; set; }
        [MaxLength(1000)]
        public string? Comment { get; set; }
        public string? FileUrl { get; set; }
        public bool CheckEdit { get; set; }
        public DateTime? DateCreated { get; set; }
        //FK
        public Guid TasksId { get; set; }
        public virtual Tasks? Tasks { get; set; }
    }
}
