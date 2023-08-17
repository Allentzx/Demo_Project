using System;
using System.Threading.Tasks;
using ITC.Core.Model;
using ITC.Data.Enum;
using ITC.Data.Utilities.Paging.PaginationModel;

namespace ITC.Core.Interface
{
	public interface IProjectService
	{
        Task<ResultModel> CreateProject(ProjectCreateModel model);
        Task<ResultModel> DeleteProject(Guid id);
        Task<ResultModel> GetAllProject();
        Task<ResultModel> UpdateProject(Guid Id, ProjectUpdateModel model);
        Task<ResultModel> GetDetailProject(Guid Id);
        Task<ResultModel> SearchProject(string keyword);
        Task<ResultModel> AssignStaffIntoProject(Guid Id, List<Guid>? staffId);
        Task<ResultModel> UnAssignStaffIntoProject(Guid Id, Guid StaffId);
        Task<ResultModel> GetJoinProject();
        Task<ResultModel> GetJoinProjectId(Guid projectId);
        Task<ResultModel> ChangeStatusProject(ChangeStatusProject model);
    }
}

