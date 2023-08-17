using System;
using ITC.Core.Model;

namespace ITC.Core.Interface
{
    public interface IPhaseService
    {
        Task<ResultModel> CreatePhase(PhaseCreateModel model);
        Task<ResultModel> DeletePhase(int Id);
        Task<ResultModel> GetAllPhase();
        Task<ResultModel> UpdateLPhase(int Id, PhaseUpdateModel model);
        Task<ResultModel> GetPhaseById(int Id);
        Task<ResultModel> UpdateDatePhase(UpdateDatePhase model);
        Task<ResultModel> RemovePhaseInProject(Guid projectId, int phaseId);
        Task<ResultModel> AddPhaseIntoProject(Guid projectId, AssignPhase model);
        Task<ResultModel> GetPhaseByProjectId(Guid? projectId);
        Task<ResultModel> UpdateStatusPhase(PhaseUpdateStatusModel model);

    }
}

