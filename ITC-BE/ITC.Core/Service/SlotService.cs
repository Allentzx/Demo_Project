using System;
using AutoMapper;
using ITC.Core.Data;
using ITC.Core.Interface;
using ITC.Core.Model;
using ITC.Data.Entities;
using ITC.Data.Enum;
using Microsoft.EntityFrameworkCore;

namespace ITC.Core.Service
{
    public class SlotService : ISlotService
    {
        private readonly ITCDBContext _context;
        private readonly IMapper _mapper;
        public SlotService(ITCDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ResultModel> CreateSlot(CreateSlotModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var s = await _context.Slot.ToListAsync();
                var co = from n in _context.Slot
                         where n.SyllabusId == model.SyllabusId
                         select n;
                int num = co.Count();
                if (co == null)
                {
                    var slot = new Slot
                    {
                        Name = model.Name,
                        Detail = model.Detail,
                        TimeAllocation = model.TimeAllocation,
                        Session = 1,
                        Type = model.Type,
                        Status = true,
                        SlotStatus = SlotEnum.New,
                        DateCreated = DateTime.Now,
                        SyllabusId = model.SyllabusId
                    };
                    await _context.Slot.AddAsync(slot);

                }
                if (co != null)
                {
                    var newslot = new Slot
                    {
                        Name = model.Name,
                        Detail = model.Detail,
                        TimeAllocation = model.TimeAllocation,
                        Session = num + 1,
                        Type = model.Type,
                        Status = true,
                        SlotStatus = SlotEnum.New,
                        DateCreated = DateTime.Now,
                        SyllabusId = model.SyllabusId
                    };
                    await _context.Slot.AddAsync(newslot);
                }

                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Code = 200;
                await transaction.CommitAsync();

            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> GetAllSlot()
        {
            var result = new ResultModel();
            try
            {
                var slots = _context.Slot.Include(x => x.Syllabus);

                if (slots == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Slot Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Slot.Include(x => x.Syllabus)
                                                         .Include(r => r.Reasons).ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetDetailSlot(int Id)
        {
            var result = new ResultModel();
            try
            {
                var slots = _context.Slot.Include(x => x.Syllabus)
                                             .Where(x => x.Id == Id);

                if (slots == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Slot Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Slot.Include(x => x.Syllabus).Include(r => r.Reasons).Where(x => x.Id == Id).ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> UpdateSlot(int Id, UpdateSlotModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var slot = await _context.Slot.FindAsync(Id);
                if (slot == null)
                {
                    result.Code = 400;
                    result.IsSuccess = true;
                    result.ResponseSuccess = "can't found slot";
                    return result;
                }

                slot.Name = model.Name;
                slot.Type = model.Type;
                slot.Detail = model.Detail;
                slot.TimeAllocation = model.TimeAllocation;
                slot.SyllabusId = model.SyllabusId;
                _context.Slot.Update(slot);
                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Code = 200;
                await transaction.CommitAsync();
                result.ResponseSuccess = await _context.Slot.Where(x => x.Id == Id).ToListAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }


        public async Task<ResultModel> DeleteSlot(int Id)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var slot = await _context.Slot.FirstOrDefaultAsync(x => x.Id == Id);

                if (slot == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = $"Slot with id: {Id} not existed!!";
                    return result;
                }
                slot.Status = false;
                _context.Slot.Update(slot);
                await _context.SaveChangesAsync();

                result.Code = 200;
                result.ResponseSuccess = "Succesfull";
                result.IsSuccess = true;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            await transaction.CommitAsync();
            return result;
        }
        public async Task<ResultModel> UpdateSlotStatus(int Id, UpdateSlotStatus model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var slot = await _context.Slot.FindAsync(Id);
                if (slot == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = "can't find slot ${Id}";
                    return result;
                }

                slot.SlotStatus = model.Status;
                _context.Slot.Update(slot);
                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Code = 200;
                await transaction.CommitAsync();
                result.ResponseSuccess = await _context.Slot.Where(x => x.Id == Id).ToListAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }
    }
}

