using ITC.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace ITC.Data.Entities
{
    public class Tasks
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        [MaxLength(255)]
        public string? TaskName { get; set; }
        public string? Description { get; set; }
        public string? Creater { get; set; }
        public DateTime DeadLine { get; set; }
        public DateTime DateCreated { get; set; }
        public TaskStatusEnum? Status { get; set; }
        public TaskStateEnum? State { get; set; }

        public Tasks? ParentTask { get; set; }
        public ICollection<Tasks>? ChildrenTask { get; set; }

        //Fk
        public Guid? ProjectId { get; set; }
        public int? PhaseId { get; set; }
        public virtual Project? Project { get; set; }
        public virtual ProjectPhase? ProjectPhase { get; set; }

        public virtual ICollection<CommentTask>? CommentTasks { get; set; }
        public virtual ICollection<AssignTask>? AssignTasks { get; set; }


    }
}
