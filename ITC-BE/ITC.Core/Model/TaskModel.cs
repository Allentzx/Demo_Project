using ITC.Data.Entities;
using ITC.Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Core.Model
{
    public class TaskModel
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public string? TaskName { get; set; }
        public string? Description { get; set; }
        public string? Creater { get; set; }
        public DateTime DeadLine { get; set; }
        public DateTime DateCreated { get; set; }
        public TaskStatusEnum? Status { get; set; }
        public TaskStateEnum? State { get; set; }
        public TaskModel? ParentTask { get; set; }
        public ICollection<TaskModel>? ChildrenTask { get; set; }
        public Guid? ProjectId { get; set; }
        public int? PhaseId { get; set; }
        public PhaseViewModel? Phase { get; set; }
    }

    public class TaskViewModel : TaskModel
    {
        public Guid Id { get; set; }
        public ProjecViewModel? Project { get; set; }
    }

    public class TaskCreateModel
    {
        public string? TaskName { get; set; }
        public Guid? ParentTaskId { get; set; }
        public string? Description { get; set; }
        public DateTime DeadLine { get; set; }
        public DateTime DateEnd { get; set; }
        public TaskStateEnum? State { get; set; }
        public TaskStatusEnum? Status { get; set; }

        public Guid? ProjectId { get;  set; }
        public int? PhaseId { get; set; }
        public List<Guid>? StaffId { get; set; }
    }

    public class ChangeStatus
    {
        [Required]
        public Guid? TaskId { get; set; }
        [Required]
        public TaskStatusEnum? TaskStatus { get; set; }
    }

    public class ChangeState
    {
        [Required]
        public Guid? TaskId { get; set; }
        [Required]
        public TaskStateEnum? State { get; set; }
    }

    public class TaskUpdateModel
    {
        public string? TaskName { get; set; }
        public string? Description { get; set; }
        public DateTime DeadLine { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateEnd { get; set; }
        public TaskStateEnum? State { get; set; }
        public TaskStatusEnum? Status { get; set; }
        public Guid? ProjectId { get; set; }
        public int? PhaseId { get; set; }
    }
}
