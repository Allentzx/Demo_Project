using System;
using ITC.Core.Model;

namespace ITC.Core.Interface
{
	public interface IAuditableEntity
	{

        string CreatedById { get; set; }
        string CreatedBy { get; set; }
        DateTimeOffset CreatedAt { get; set; }
        string LastModifiedById { get; set; }
        string LastModifiedBy { get; set; }
        DateTimeOffset? LastModifiedAt { get; set; }
    }

    public interface IAuditService
    {
        Task<ResultModel> GetAllChangeLog();
        Task<ResultModel> GetDetailChangeLog(int Id);
        Task<ResultModel> GetChangeLogTasks();
    }
}

