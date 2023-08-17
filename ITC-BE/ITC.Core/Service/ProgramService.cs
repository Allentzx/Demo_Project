using System;
using AutoMapper;
using ITC.Core.Data;
using ITC.Core.Interface;
using ITC.Core.Model;
using ITC.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ITC.Core.Service
{
	public class ProgramService : IProgramService
    {
        private readonly ITCDBContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;
        public ProgramService(ITCDBContext context, IMapper mapper, IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }
        public async Task<ResultModel> CreateProgram(CreateProgramModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {

                var Program = new Program
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    Description = model.Description,
                    Status = true,
                };

                await _context.Program.AddAsync(Program);
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

        public async Task<ResultModel> GetAllProgram()
        {
            var result = new ResultModel();
            try
            {
                var Program = _context.Program;

                if (Program == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Program Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Program.ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetDetailProgram(Guid Id)
        {
            var result = new ResultModel();
            try
            {
                var Program = _context.Program.Where(x => x.Id == Id);
                if (Program == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Program Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Program
                                             .Where(x => x.Id == Id).ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }


        public async Task<ResultModel> UpdateProgram(Guid Id, UpdateProgramModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var Program = await _context.Program.FindAsync(Id);
                if (Program == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = "Can't found Program";
                    return result;
                }

                Program.Name = model.Name;
                Program.Status = model.Status;
                Program.Description = model.Description;
                _context.Program.Update(Program);
                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Code = 200;
                await transaction.CommitAsync();
                result.ResponseSuccess = await _context.Program.Where(x => x.Id == Id).ToListAsync(); ;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> DeleteProgram(Guid Id)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var Program = await _context.Program.FirstOrDefaultAsync(x => x.Id == Id);

                if (Program == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = $"Program with id: {Id} not existed!!";
                    return result;
                }
                Program.Status = false;

                _context.Program.Update(Program);
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
    }
}


