using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITC.Data.Entities
{
    public class Post
    {
        [Key]
        public Guid Id { get; set; }
        public string? Author { get; set; }
        public string? Title { get; set; }
        public string? SubTitle { get; set; } 
        public string? PosterUrl { get; set; }
        public string? Content { get; set; }
        public DateTime DateCreated  { get; set; }
        public bool Status { get; set; }

        //Fk
        public Guid StaffId { get; set; }
        public virtual Staff? Staffs { get; set; }
        public ICollection<PostImage>? PostImages { get; set; }
    }
}
