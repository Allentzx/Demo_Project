using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ITC.Data.Enum;

namespace ITC.Data.Entities
{
    public class Slot
    {
        [Key]
        public int Id { get; set; }
        public int? Session { get; set; }
        [StringLength(200)]
        public string? Name { get; set; }
        [StringLength(1000)]
        public string? Detail { get; set; }
        public string? TimeAllocation { get; set; }
        public SlotEnum SlotStatus { get; set; }
        public bool Status { get; set; }
        public string? Type { get; set; }
        public DateTime DateCreated { get; set; }

        //Fk
        public Guid SyllabusId { get; set; }

        public virtual Syllabus? Syllabus { get; set; }
        public ICollection<Reason>? Reasons { get; set; }
    }
}
