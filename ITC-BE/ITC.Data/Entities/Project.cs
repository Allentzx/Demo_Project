using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ITC.Data.Enum;

namespace ITC.Data.Entities
{
    public class Project
    {
        [Key]
        public Guid Id { get; set; }
        [MaxLength(100)]
        public string? ProjectName { get; set; }
        [MaxLength(100)]
        public string? CampusName { get; set; }
        [MaxLength(1000)]
        public string? Description { get; set; }
       
        public DateTime? EstimateTimeStart { get; set; }
        public DateTime? EstimateTimeEnd{ get; set; }
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
        public Guid? ProgramId { get; set;}

        // 1-object

        public virtual Account? Creater { get; set; }
        public virtual Course? Course { get; set; }
        public virtual Partner? Partner { get; set; }
        public virtual Campus? Campus { get; set; }
        public virtual Program? Program { get; set; }
        public virtual ConfigProject? ConfigProject { get; set; }

        //array obj
        public ICollection<JoinProject>? JoinProjects { get; set; }
        public ICollection<Registration>? Registrations { get; set; }
        public ICollection<Tasks>? Tasks { get; set; }
        public CancelProject? CancelProjects { get; set; }
        public ICollection<Document>? Documents { get; set; }
        public ICollection<ProjectPhase>? ProjectPhase { get; set; }

    }
}
