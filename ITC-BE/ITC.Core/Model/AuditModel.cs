using System;
namespace ITC.Core.Model
{
    public class AuditModel
    {
        public Guid? UserId { get; set; }
        public string? Type { get; set; }
        public string? TableName { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? AffectedColumns { get; set; }
        public string? PrimaryKey { get; set; }
    }

    public class AuditNewModel
    {
        public Guid? UserId { get; set; }
        public string? Type { get; set; }
        public string? TableName { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public Object? OldValues { get; set; }
        public Object? NewValues { get; set; }
        public string? AffectedColumns { get; set; }
        public string? PrimaryKey { get; set; }
    }

    public class TaskChangeLogModel
    {

        public string? Creater { get; set; }
        public DateTime? DateCreated { get; set; }
        public string? Type { get; set; }
        public DateTime? DeadLine { get; set; }
        public string? Description { get; set; }
        public string? ParentId { get; set; }
        public string? ProjectId { get; set; }
        public string? ProjectPhaseId { get; set; }
        public bool State { get; set; }
        public bool Status { get; set; }
        public string? TaskName { get; set; }

    }
}

