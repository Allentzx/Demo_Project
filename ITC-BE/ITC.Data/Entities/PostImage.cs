using System;
using System.ComponentModel.DataAnnotations;

namespace ITC.Data.Entities
{
    public class PostImage
    {
        [Key]
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public string? PostImageUrl { get; set; }
        public virtual Post? Post { get; set; }
    }
}

