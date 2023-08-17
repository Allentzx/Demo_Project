using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Data.Entities
{
    public class Syllabus
    {
        [Key]
        public Guid Id { get; set; }
        [MaxLength(25)]
        public string? Content { get; set; }
        [MaxLength(1000)]
        public string? Description { get; set; }
        public string? Note { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Status { get; set; }

        //Fk
        public Guid CourseId { get; set; }
        public Guid? PartnerId { get; set; }
        public virtual Course? Course { get; set; }
        public virtual Partner? Partner { get; set; }
        public ICollection<Slot>? Slots { get; set; }
    }
}
