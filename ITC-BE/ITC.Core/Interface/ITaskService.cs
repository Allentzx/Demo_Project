using ITC.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Core.Interface
{
    public interface ITaskService
    {
        Task<ResultModel> CreateTask(TaskCreateModel model);
        Task<ResultModel> DisableTask(Guid id);
        Task<ResultModel> UpdateTask(Guid Id, TaskUpdateModel model);
        Task<ResultModel> AssignTask(Guid TasksId, Guid StaffId);
        Task<ResultModel> UnAssignTask(Guid Id, Guid StaffId);
        Task<ResultModel> GetDetailTask(Guid Id);
        Task<ResultModel> GetRootTask();
        Task<ResultModel> ChangeStatusTask(ChangeStatus model);
        Task<ResultModel> ChangeStateTask(ChangeState model);
        Task<ResultModel> SearchTask(string keyword);
        Task<ResultModel> GetChildTask(Guid parentId);
        Task<ResultModel> GetAllTask();
        Task<ResultModel> GetTaskByProjectId(Guid projectId);
    }
}
