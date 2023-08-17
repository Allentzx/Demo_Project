using System;
using AutoMapper;
using DocumentFormat.OpenXml.InkML;
using ITC.Core.Data;
using ITC.Core.Interface;
using ITC.Core.Model;
using ITC.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ITC.Core.Service
{
    public class SyllabusService : ISyllabusService
    {
        private readonly ITCDBContext _context;
        private readonly IMapper _mapper;
        public SyllabusService(ITCDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ResultModel> CreateSyllabus(CreateSyllabusModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var syl = _context.Syllabus;
                if (syl == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Syllabus Not Found!";
                    return result;
                }

                var syllabus = new Syllabus
                {
                    Id = Guid.NewGuid(),
                    Content = model.Content,
                    Description = model.Description,
                    Note =model.Note,
                    DateCreated = DateTime.Now,
                    Status = true,
                    CourseId = model.CourseId,
                    PartnerId = model.PartnerId
                };

                await _context.Syllabus.AddAsync(syllabus);
                await _context.SaveChangesAsync();

                result.IsSuccess = true;
                result.Code = 200;
                result.ResponseSuccess = syllabus;
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

        public async Task<ResultModel> GetAllSyllabus()
        {
            var result = new ResultModel();
            try
            {
                var syllabus = _context.Syllabus.Include(x => x.Course)
                                                .Include(x => x.Slots)
                                                .ThenInclude(r => r.Reasons).ThenInclude(a => a.Deputies);



                if (syllabus == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any syllabus Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = _mapper.ProjectTo<SyllabusViewModel>(syllabus).ToList();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetDetailSyllabus(Guid Id)
        {
            var result = new ResultModel();
            try
            {
                var syllabus = await _context.Syllabus.Include(x => x.Course)
                                                      .Include(x => x.Slots)
                                                      .ThenInclude(r => r.Reasons).ThenInclude(a => a.Deputies)
                                                      .Where(x => x.Id == Id).ToListAsync();


                if (syllabus == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Course Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = syllabus;

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> UpdateSyllabus(Guid Id, UpdateSyllabusModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var syllabus = await _context.Syllabus.FindAsync(Id);
                if (syllabus == null)
                {
                    result.Code = 400;
                    result.IsSuccess = true;
                    result.ResponseSuccess = "Cant found Syllabus";
                    return result;
                }

                syllabus.Content = model.Content;
                syllabus.Description = model.Description;
                syllabus.Note = model.Note;
                syllabus.Status = model.Status;
                syllabus.CourseId = model.CourseId;
                syllabus.PartnerId = model.PartnerId;
                _context.Syllabus.Update(syllabus);
                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Code = 200;
                await transaction.CommitAsync();
                result.ResponseSuccess = await _context.Syllabus.Where(x => x.Id == Id).ToListAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }


        public async Task<ResultModel> DeleteSyllabus(Guid Id)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var syll = await _context.Syllabus.FirstOrDefaultAsync(x => x.Id == Id);

                if (syll == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = $"Syllabus with id: {Id} not existed!!";
                    return result;
                }
                _context.Syllabus.Remove(syll);
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

        public async Task<ResultModel> UpdateSyllabusStatus(Guid Id, UpdateSyllabusStatusModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var syllabus = await _context.Syllabus.FindAsync(Id);
                if (syllabus == null)
                {
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.ResponseSuccess = new SyllabusViewModel();
                    return result;
                }
                syllabus.Status = model.Status;
                _context.Syllabus.Update(syllabus);
                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Code = 200;
                await transaction.CommitAsync();
                result.ResponseSuccess = await _context.Syllabus.Where(x => x.Id == Id).ToListAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> SearchSyllabus(string keyword)
        {
            var result = new ResultModel();
            try
            {
                var syllabus = await _context.Syllabus.Include(x => x.Course).Where(b => b.Course.Activity.Contains(keyword) || b.Course.Content.Contains(keyword)).ToListAsync();
                if (!syllabus.Any())
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = "not found";
                    return result;
                }

                result.IsSuccess = true;
                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = syllabus;

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }


        public async Task<ResultModel> GetListSyllabusPartner(Guid? PartnerId)
        {
            var result = new ResultModel();
            try
            {

                var syllabus = await _context.Syllabus.Where(x => x.PartnerId == PartnerId)
                    .Include(x => x.Slots).ThenInclude(r => r.Reasons).ThenInclude(a => a.Deputies).ThenInclude(x=>x.Account)
                    .Include(x=>x.Course)
                    .ToListAsync();

                if (syllabus == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Course Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = _mapper.Map<List<SyllabusViewModel>>(syllabus); 

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }
    }
}

