using System;
using ITC.Core.Model;

namespace ITC.Core.Interface
{
	public interface IProgramService
	{
        Task<ResultModel> CreateProgram(CreateProgramModel model);
        Task<ResultModel> GetAllProgram();
        Task<ResultModel> GetDetailProgram(Guid Id);
        Task<ResultModel> DeleteProgram(Guid Id);
        Task<ResultModel> UpdateProgram(Guid Id, UpdateProgramModel model);
    }
}

