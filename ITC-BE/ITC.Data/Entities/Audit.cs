using System;
namespace ITC.Data.Entities
{
	public class Audit
	{
        public int Id { get; set; }
        public Guid? UserId { get; set; }
        public string? FullName { get; set; }

        public string? Type { get; set; }
        public string? TableName { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? AffectedColumns { get; set; }
        public string? PrimaryKey { get; set; }
    }
}

