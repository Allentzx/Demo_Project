using System;
using ITC.Core.Model;

namespace ITC.Core.Interface
{
	public interface IRegistrationService
    {
        Task<ResultModel> CreateNew(CreateRegistrationModel model);
        Task<ResultModel> GetAll();
        Task<ResultModel> GetDetailResByStudentId(Guid studentId);
        Task<ResultModel> UpdateRegisByStudentId(Guid studentId, UpdateRegisInfoModel model);
        Task<ResultModel> GetRootRegis();
        Task<ResultModel> GetDetailResByProjectId(Guid projectId);
        Task<ResultModel> GetDetailResId(Guid Id);
        Task<ResultModel> UpdateRegisId(Guid Id, UpdateRegisModel model);
        Task<ResultModel> UpdateRegisStatus(Guid Id, bool status);
        Task<ResultModel> GetChildReg(Guid parentId);
        Task<ResultModel> CreateNewOptional(CreateOptionalModel model);
        Task<ResultModel> DeleteOption(Guid resId, Guid optionId);
        Task<ResultModel> UpdateQuestion(UpdateQuestionModel model);
        Task<ResultModel> UpdateAnswer(UpdateAnswerModel model);

    }
}

