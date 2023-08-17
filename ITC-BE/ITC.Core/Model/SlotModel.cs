using System;
using ITC.Data.Entities;
using ITC.Data.Enum;

namespace ITC.Core.Model
{
    public class SlotModel
    {
        public int? Session { get; set; }
        public string? Name { get; set; }
        public string? Detail { get; set; }
        public string? TimeAllocation { get; set; }
        public SlotEnum SlotStatus { get; set; }
        public bool Status { get; set; }
        public string? Type { get; set; }
        public DateTime DateCreated { get; set; }
        public Guid SyllabusId { get; set; }
    }
    public class SlotViewModel : SlotModel
    {

        public int Id { get; set; }
        public virtual SyllabusViewModel? Syllabus { get; set; }
    }

    public class ViewSlotModel
    {
        public int Id { get; set; }   
        public int? Session { get; set; }
        public string? Name { get; set; }
        public string? Detail { get; set; }
        public string? TimeAllocation { get; set; }
        public SlotEnum SlotStatus { get; set; }
        public bool Status { get; set; }
        public string? Type { get; set; }
        public DateTime DateCreated { get; set; }
        public Guid SyllabusId { get; set; }
        public ICollection<ReasonViewModel>? Reasons { get; set; }
    }

   

    

    public class CreateSlotModel
    {
        public string? Name { get; set; }
        public string? Detail { get; set; }

        public string? TimeAllocation { get; set; }
        public string? Type { get; set; }
        public Guid SyllabusId { get; set; }

    }

    public class UpdateSlotModel
    {
        public string? Name { get; set; }
        public string? Detail { get; set; }

        public string? TimeAllocation { get; set; }

        public string? Type { get; set; }
        public Guid SyllabusId { get; set; }
    }

    public class UpdateSlotStatus
    {
        public SlotEnum Status { get; set; }
    }

}

