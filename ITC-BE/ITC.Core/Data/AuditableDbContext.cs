using System;
using ITC.Data.Entities;
using ITC.Data.Enum;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ITC.Core.Data
{
    public class AuditableDbContext : IdentityDbContext
    {
        protected AuditableDbContext(DbContextOptions options) : base(options) { }

        // ReSharper disable once MemberCanBePrivate.Global
        public DbSet<Audit>? AuditTrails { get; set; }

        protected async Task<int> SaveChangesAsync(Guid? userId = null, string? fullName = null ,CancellationToken cancellationToken = new())
        {
            if (AuditTrails is null)
            {
                throw new NullReferenceException("DbSet<Audit> is null");
            }

            var auditEntries = OnBeforeSaveChanges(userId, fullName);
            var result = await base.SaveChangesAsync(cancellationToken);

            await OnAfterSaveChanges(auditEntries, cancellationToken);

            return result;
        }

        private List<AuditEntry> OnBeforeSaveChanges(Guid? userId, string? fullName)
        {
            ChangeTracker.DetectChanges();

            var auditEntries = new List<AuditEntry>();

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Audit || entry.State is EntityState.Detached or EntityState.Unchanged)
                {
                    continue;
                }

                var auditEntry = new AuditEntry(entry)
                {
                    TableName = entry.Entity.GetType().Name,
                    UserId = userId,
                    FullName = fullName
                };

                auditEntries.Add(auditEntry);

                foreach (var property in entry.Properties)
                {
                    if (property.IsTemporary)
                    {
                        auditEntry.TemporaryProperties.Add(property);
                        continue;
                    }

                    var propertyName = property.Metadata.Name;

                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue!;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.AuditType = AuditType.Create;
                            auditEntry.NewValues[propertyName] = property.CurrentValue!;
                            break;

                        case EntityState.Deleted:
                            auditEntry.AuditType = AuditType.Delete;
                            auditEntry.OldValues[propertyName] = property.OriginalValue!;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified && property.OriginalValue?.Equals(property.CurrentValue) == false)
                            {
                                auditEntry.ChangedColumns.Add(propertyName);
                                auditEntry.AuditType = AuditType.Update;
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue!;
                            }

                            break;

                        case EntityState.Detached:
                        case EntityState.Unchanged:
                        default:
                            break;
                    }
                }
            }

            foreach (var auditEntry in auditEntries.Where(x => !x.HasTemporaryProperties))
            {
                AuditTrails?.Add(auditEntry.ToAudit());
            }

            return auditEntries.Where(x => x.HasTemporaryProperties).ToList();
        }

        private Task OnAfterSaveChanges(List<AuditEntry>? auditEntries, CancellationToken cancellationToken = new())
        {
            if (auditEntries == null || auditEntries.Count == 0)
            {
                return Task.CompletedTask;
            }

            foreach (var auditEntry in auditEntries)
            {
                foreach (var prop in auditEntry.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue!;
                    }
                    else
                    {
                        auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue!;
                    }
                }

                AuditTrails?.Add(auditEntry.ToAudit());
            }

            return SaveChangesAsync(cancellationToken);
        }
    }
}

