using System;
using System.Security.Claims;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using ITC.Core.Data;
using ITC.Core.Interface;
using ITC.Core.Model;
using ITC.Data.Entities;
using ITC.Data.Enum;
using Microsoft.EntityFrameworkCore;

namespace ITC.Core.Service
{
    public class CourseService : ICourseService
    {
        private readonly ITCDBContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;
        public CourseService(ITCDBContext context, IMapper mapper, IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }
        public async Task<ResultModel> CreateCourse(CreateCourseModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {

                var CheckName = await _context.Course.Where(x => x.CourseName == model.CourseName).FirstOrDefaultAsync();
                if (CheckName != null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = $"CourseName : {model.CourseName}  existed!!";
                    return result;
                };

                var course = new Course
                {
                    Id = Guid.NewGuid(),
                    Activity = model.Activity,
                    Content = model.Content,
                    CourseName = model.CourseName,
                    DateCreated = DateTime.Now,
                    Status = true,
                    Creator = _context.Account.Where(x => x.AccountId == _userContextService.UserID).FirstOrDefault(),
                };

                await _context.Course.AddAsync(course);
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

        public async Task<ResultModel> GetAllCourse()
        {
            var result = new ResultModel();
            try
            {
                var course = _context.Course.Include(x => x.Syllabus);

                if (course == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any course Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Course.Include(x => x.Syllabus).ThenInclude(y => y.Slots).ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetDetailCourse(Guid Id)
        {
            var result = new ResultModel();
            try
            {
                var course = await _context.Course.Include(x => x.Syllabus).ThenInclude(p => p.Partner)
                                                                .Include(x => x.Syllabus).ThenInclude(y => y.Slots)
                                                                .Include(x => x.Syllabus).ThenInclude(y => y.Partner)
                                                                .Where(x => x.Id == Id).ToListAsync(); 
                if (course == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Course Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = course;
                //result.ResponseSuccess= _mapper.Map<List<ViewCourseDetailModel>>(course);
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }


        public async Task<ResultModel> UpdateCourse(Guid Id, UpdateCourseModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var course = await _context.Course.FindAsync(Id);
                if (course == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = $"Course with id: {Id} not existed!!";
                    return result;
                }
                var CheckName = await _context.Course.Where(x => x.CourseName == model.CourseName).FirstOrDefaultAsync();
                if (CheckName != null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = $"CourseName : {model.CourseName}  existed!!";
                    return result;
                };

                course.Content = model.Content;
                course.Status = model.Status;
                course.Activity = model.Activity;
                if (model.CourseName != null)
                {
                    course.CourseName = model.CourseName;
                }
                course.DateCreated = model.DateCreate;
                course.Creator = _context.Account.Where(x => x.AccountId == _userContextService.UserID).FirstOrDefault();
                _context.Course.Update(course);
                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Code = 200;
                await transaction.CommitAsync();
                result.ResponseSuccess = await _context.Course.Where(x => x.Id == Id).ToListAsync(); ;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> DeleteCourse(Guid Id)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var course = await _context.Course.FirstOrDefaultAsync(x => x.Id == Id);
                var projectstatus = await _context.Project.Where(x => x.ProjectStatus == ProjectStatusEnum.Canceled
                || x.ProjectStatus == ProjectStatusEnum.InActive && x.CourseId == Id).FirstOrDefaultAsync();
                if(projectstatus == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = $"course with id: {course.CourseName} can't disalbe when project running!!";
                    return result;
                }
                if (course == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = $"course with id: {Id} not existed!!";
                    return result;
                }
                course.Status = false;

                _context.Course.Update(course);
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


