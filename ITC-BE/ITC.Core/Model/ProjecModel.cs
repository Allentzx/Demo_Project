using System;
using System.ComponentModel.DataAnnotations;
using ITC.Data.Enum;

namespace ITC.Core.Model
{
    public class ProjecModel
    {
        public string? ProjectName { get; set; }
        public string? CampusName { get; set; }
        public string? Description { get; set; }

        public string? Creater { get; set; }
        public bool Status { get; set; }
        public DateTime? EstimateTimeStart { get; set; }
        public DateTime? EstimateTimeEnd { get; set; }
        public DateTime? OfficalTimeStart { get; set; }
        public DateTime? OfficalTimeEnd { get; set; }
        public DateTime? DateCreate { get; set; }
        public DateTime? DateOpenRegis { get; set; }
        public DateTime? DateCloseRegis { get; set; }
        public ProjectStatusEnum? ProjectStatus { get; set; }
        public bool CheckNegotiationStatus { get; set; }


        public Guid LeaderId { get; set; }
        public Guid CourseId { get; set; }
        public Guid PartnerId { get; set; }
        public Guid ConfigId { get; set; }

    }

    public class ProjecViewModel : ProjecModel
    {
        public Guid Id { get; set; }
        public CourseViewModel? Course { get; set; }
        public ICollection<Task>? EmpTasks { get; set; }
    }

    public class ProjecDTOModel
    {
        public Guid Id { get; set; }
        public string? ProjectName { get; set; }
        public string? CampusName { get; set; }
        public string? Description { get; set; }

        public bool Status { get; set; }
        public DateTime? EstimateTimeStart { get; set; }
        public DateTime? EstimateTimeEnd { get; set; }
        public DateTime? OfficalTimeStart { get; set; }
        public DateTime? OfficalTimeEnd { get; set; }
        public DateTime? DateCreated { get; set; }
        public ProjectStatusEnum? ProjectStatus { get; set; }
        public bool CheckNegotiationStatus { get; set; }

        //key
        public Guid LeaderId { get; set; }
        public Guid CourseId { get; set; }
        public Guid PartnerId { get; set; }
        public Guid? CampusId { get; set; }
        public Guid? ConfigProjectId { get; set; }
        public Guid? ProgramId { get; set; }

        // 1-object

        public virtual ViewCurrentAccountModel? Creater { get; set; }
        public virtual ViewCourseModel? Course { get; set; }
        public virtual PartnerViewModel? Partner { get; set; }
        public virtual CampusViewModel? Campus { get; set; }
        public virtual ProgramModel? Program { get; set; }

        //array obj
        //public ICollection<ViewRegistrationProjectModel>? Registrations { get; set; }
        public ICollection<TaskModel>? Tasks { get; set; }
        //public ICollection<CancelModel>? CancelProjects { get; set; }
        //public ICollection<Document>? Documents { get; set; }
        public ICollection<ProjectPhaseModel>? ProjectPhase { get; set; }
    }

    public class ProjectCreateModel
    {
        public string? CampusName { get; set; }
        public string? ProjectName { get; set; }
        public string? Description { get; set; }

        public DateTime? EstimateTimeStart { get; set; }
        public DateTime? EstimateTimeEnd { get; set; }
        public DateTime? OfficalTimeStart { get; set; }
        public DateTime? OfficalTimeEnd { get; set; }
        //Fk
        public List<int>? PhaseId { get; set; }
        public Guid LeaderId { get; set; }
        public Guid? ProgramId { get; set; }
        public Guid CourseId { get; set; }
        public Guid PartnerId { get; set; }
        public Guid? CampusId { get; set; }
        //public Guid? ConfigId { get; set; }
    }


    public class ProjectUpdateModel
    {
        public string? CampusName { get; set; }
        public string? ProjectName { get; set; }
        public string? Description { get; set; }

        public string? Creater { get; set; }
        public bool Status { get; set; }
        public DateTime? EstimateTimeStart { get; set; }
        public DateTime? EstimateTimeEnd { get; set; }
        public DateTime? OfficalTimeStart { get; set; }
        public DateTime? OfficalTimeEnd { get; set; }
        public DateTime? DateCreate { get; set; }
        public ProjectStatusEnum? ProjectStatus { get; set; }
        public bool CheckNegotiationStatus { get; set; }
        public Guid LeaderId { get; set; }
        public Guid CourseId { get; set; }
        public Guid PartnerId { get; set; }
        public Guid? ProgramId { get; set; }
        public Guid? CampusId { get; set; }
        public Guid? ConfigId { get; set; }
    }

    public class ChangeStatusProject
    {
        [Required]
        public Guid? ProjectId { get; set; }
        [Required]
        public ProjectStatusEnum? Status { get; set; }
    }

}

