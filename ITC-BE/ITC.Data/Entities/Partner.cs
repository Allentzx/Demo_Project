using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Data.Entities
{
    public class Partner
    {
        [Key]
        public Guid Id { get; set; }
        [MaxLength(25)]
        public string? Name { get; set; }
        public string? Local { get; set; }
        public string? Note { get; set; }
        public bool Status { get; set; }


        public ICollection<Deputy>? Deputies { get; set; }
        public ICollection<Campus>? Campuses { get; set; }
        public ICollection<Project>? Projects { get; set; }
        public ICollection<Syllabus>? Syllabus { get; set; }

    }
}
