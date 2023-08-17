using System;
namespace ITC.Core.Model
{
	public class SyllabusModel
	{
        public string? Content { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Status { get; set; }
        public Guid? PartnerId { get; set; }

    }

    public class SyllabusViewModel : SyllabusModel
    {

        public Guid Id { get; set; }
        public ViewCourseModel? Course { get; set; }
        public ICollection<ViewSlotModel>? Slots { get; set; }
    }

    public class SyllabusViewCourseModel : SyllabusModel
    {

        public Guid Id { get; set; }
        public virtual PartnerViewModel? Partner { get; set; }
        public ICollection<ViewSlotModel>? Slots { get; set; }
    }

    public class CreateSyllabusModel
    {
        public string? Content { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public Guid CourseId { get; set; }
        public Guid? PartnerId { get; set; }

    }


    public class UpdateSyllabusModel
    {
        public string? Content { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool Status { get; set; }
        public Guid CourseId { get; set; }
        public Guid? PartnerId { get; set; }
    }

    public class UpdateSyllabusStatusModel
    {
        public bool Status { get; set; }
    }
}

