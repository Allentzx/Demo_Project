using System;
using System.ComponentModel.DataAnnotations;
using ITC.Data.Enum;

namespace ITC.Core.Model
{
    public class ProjectPhaseModel
    {
        public Guid? Id { get; set; }
        public DateTime? DateBegin { get; set; }
        public DateTime? DateEnd { get; set; }

        public Guid? ProjectId { get; set; }
        public int? PhaseId { get; set; }
        public bool Status { get; set; }


        public virtual PhaseViewModel? Phase { get; set; }
    }



    public class PhaseViewModel
    {
        public int? Id { get; set; }
        public string? PhaseName { get; set; }
        public bool Status { get; set; }
        public DateTime DateCreate { get; set; }
    }

    public class PhaseCreateModel
    {
        public string? PhaseName { get; set; }

    }

    public class PhaseUpdateModel
    {
        public string? PhaseName { get; set; }
        public bool Status { get; set; }
    }
    public class PhaseUpdateStatusModel
    {
        [Required]
        public Guid? ProjectId { get; set; }
        [Required]
        public int? PhaseId { get; set; }
        public bool Status { get; set; }
    }

    public class AssignPhase
    {
        public int? PhaseId { get; set; }
        public string? PhaseName { get; set; }
        public DateTime? DateBegin { get; set; }
        public DateTime? DateEnd { get; set; }
    }
    
    public class UpdateDatePhase
    {
        [Required]
        public Guid? ProjectId { get; set; }
        [Required]
        public int? PhaseId { get; set; }
        public DateTime? DateBegin { get; set; }
        public DateTime? DateEnd { get; set; }
    }

}

