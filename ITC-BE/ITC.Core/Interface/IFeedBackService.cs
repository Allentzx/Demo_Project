using System;
using ITC.Core.Model;

namespace ITC.Core.Interface
{
	public interface IFeedBackService
	{
        Task<ResultModel> CreateNew(CreateFeedbackModel model);
        Task<ResultModel> GetDetailFeedBackByStudentId(Guid studentId);
        Task<ResultModel> GetRootFeedBack();
        Task<ResultModel> GetDetailByRes(Guid resId);
        Task<ResultModel> GetDetailFbId(Guid Id);
        Task<ResultModel> UpdateFeedBackId(Guid Id, UpdateFeedbackInfoModel model);
        Task<ResultModel> GetChildFb(Guid parentId);
        Task<ResultModel> CreateFeedbacKQuestion(CreateFeedbacKQuestionModel model);
        Task<ResultModel> DeleteQuestion(Guid Id, Guid questionId);
        Task<ResultModel> UpdateQuestion(UpdateFeedBackQuestionModel model);
        Task<ResultModel> UpdateAnswer(UpdateFeedBackAnswerModel model);
    }
}

