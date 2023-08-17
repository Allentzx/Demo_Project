using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITC.Data.Entities
{
    public class Major
    {
        [Key]
        public Guid Id { get; set; }
        [MaxLength(25)]
        public string? Name { get; set; }
        public string? MajorFullName { get; set; }
        public bool Status { get; set; }

        public ICollection<Student>? Students { get; set; }
    }
}
