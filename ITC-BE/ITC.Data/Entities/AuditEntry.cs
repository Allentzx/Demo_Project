using System;
using ITC.Data.Enum;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace ITC.Data.Entities
{
    public class AuditEntry
    {
        public AuditEntry(EntityEntry entry) => Entry = entry;

        private EntityEntry Entry { get; }
        public Guid? UserId { get; init; }
        public string? FullName { get; init; }
        public string? TableName { get; init; }
        public Dictionary<string, object> KeyValues { get; } = new();
        public Dictionary<string, object> OldValues { get; } = new();
        public Dictionary<string, object> NewValues { get; } = new();
        public List<PropertyEntry> TemporaryProperties { get; } = new();
        public AuditType AuditType { get; set; }
        public List<string> ChangedColumns { get; } = new();

        public bool HasTemporaryProperties
        {
            get => TemporaryProperties.Any();
        }

        public Audit ToAudit()
        {
            var audit = new Audit
            {
                UserId = UserId,
                FullName = FullName,
                Type = AuditType.ToString(),
                TableName = TableName,
                DateTime = DateTime.UtcNow,
                PrimaryKey = JsonConvert.SerializeObject(KeyValues),
                OldValues = OldValues.Count == 0 ? null : JsonConvert.SerializeObject(OldValues),
                NewValues = NewValues.Count == 0 ? null : JsonConvert.SerializeObject(NewValues),
                AffectedColumns = ChangedColumns.Count == 0
                    ? null
                    : JsonConvert.SerializeObject(ChangedColumns)
            };

            return audit;
        }
    }
}

