using System;
using ITC.Core.Model;

namespace ITC.Core.Interface
{
	public interface ICourseService
	{
        Task<ResultModel> CreateCourse(CreateCourseModel model);
        Task<ResultModel> GetAllCourse();
        Task<ResultModel> GetDetailCourse(Guid Id);
        Task<ResultModel> DeleteCourse(Guid Id);
        Task<ResultModel> UpdateCourse(Guid Id, UpdateCourseModel model);
    }
}

