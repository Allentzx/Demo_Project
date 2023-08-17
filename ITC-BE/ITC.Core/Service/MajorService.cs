using System;
using AutoMapper;
using ITC.Core.Data;
using ITC.Core.Interface;
using ITC.Core.Model;
using ITC.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ITC.Core.Service
{
    public class MajorService : IMajorService
    {
        private readonly ITCDBContext _context;
        private readonly IMapper _mapper;
        public MajorService(ITCDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResultModel> CreateMajor(CreateMajorModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var m = _context.Major;
                if (m == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Course Not Found!";
                    return result;
                }

                var major = new Major
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    MajorFullName = model.MajorFullName,
                    Status = true

                };

                await _context.Major.AddAsync(major);
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

        public async Task<ResultModel> GetAllMajor()
        {
            var result = new ResultModel();
            try
            {
                var m = _context.Major;

                if (m == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Major Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Major.Include(x => x.Students).ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> GetDetailMajor(Guid Id)
        {
            var result = new ResultModel();
            try
            {
                var m = _context.Major.Where(x => x.Id == Id);

                if (m == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Major Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Major.Where(x => x.Id == Id)
                                                               .Include(x => x.Students).ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }



        public async Task<ResultModel> UpdateMajor(Guid Id, UpdateMajorModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var m = await _context.Major.FindAsync(Id);
                if (m == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Major Not Found!";
                    return result;
                }

                m.Name = model.Name;
                m.MajorFullName = model.MajorFullName;
                m.Status = model.Status;
                _context.Major.Update(m);
                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Code = 200;
                await transaction.CommitAsync();
                result.ResponseSuccess = await _context.Major.Where(x => x.Id == Id)
                                                               .Include(x => x.Students).ToListAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }


        public async Task<ResultModel> SearchMajor(string keyword)
        {
            var result = new ResultModel();
            try
            {
                var projects = await _context.Major.Where(x => x.Name.Contains(keyword)).ToListAsync();
                if (!projects.Any())
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = "not found";
                    return result;
                }

                result.IsSuccess = true;
                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Major.Where(x => x.Name.Contains(keyword)).ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> DeleteMajor(Guid Id)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var f = await _context.Major.FirstOrDefaultAsync(x => x.Id == Id);

                if (f == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = $"Major with id: {Id} not existed!!";
                    return result;
                }

                f.Status = false;
                _context.Major.Update(f);
                await _context.SaveChangesAsync();

                result.Code = 200;
                result.ResponseSuccess = "Major is disable";
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
    }
}

