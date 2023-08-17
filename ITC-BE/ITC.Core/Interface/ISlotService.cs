using System;
using ITC.Core.Model;

namespace ITC.Core.Interface
{
	public interface ISlotService
	{
        Task<ResultModel> CreateSlot(CreateSlotModel model);
        Task<ResultModel> GetAllSlot();
        Task<ResultModel> GetDetailSlot(int Id);
        Task<ResultModel> DeleteSlot(int Id);
        Task<ResultModel> UpdateSlot(int Id, UpdateSlotModel model);
        Task<ResultModel> UpdateSlotStatus(int Id, UpdateSlotStatus model);
    }
}

