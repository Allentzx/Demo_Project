using System;
using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using ITC.Core.Data;
using ITC.Core.Interface;
using ITC.Core.Model;
using ITC.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ITC.Core.Service
{
	public class FeedBackService : IFeedBackService
    {
        private readonly ITCDBContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public FeedBackService(ITCDBContext context, IMapper mapper
                                , IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task<ResultModel> CreateFeedbacKQuestion(CreateFeedbacKQuestionModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                foreach (var item in model.Question)
                {
                    var ques = new FeedBackAddOn
                    {
                        Id = Guid.NewGuid(),
                        FeedBackId = model.FeedBackId,
                        Question = item,
                    };

                    await _context.FeedBackAddOn.AddAsync(ques);
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

        public async Task<ResultModel> CreateNew(CreateFeedbackModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var fb = new FeedBack
                {
                    Id = Guid.NewGuid(),
                    ParenFeedBacksId = model.ParentFeedBacksId,
                    Title = model.Title,
                    Description = model.Description,
                    FeedBackContent = model.FeedBackContent,
                    DateCreated = DateTime.Now,
                    Status = true,
                    RegistrationId = model.RegistrationId,
                    
                };
                if (model.AddMoreQuestion != null)
                {
                    foreach (var item in model.AddMoreQuestion)
                    {
                        var addOption = new FeedBackAddOn
                        {
                            Id = Guid.NewGuid(),
                            FeedBackId = fb.Id,
                            Question = item,
                            Answer = null,
                        };
                        await _context.FeedBackAddOn.AddAsync(addOption);
                    }
                }
                if (model.ParentFeedBacksId != null)
                {
                    var checkParent = await _context.FeedBack.Where(x => x.Id == model.ParentFeedBacksId).FirstOrDefaultAsync();
                    if (checkParent != null)
                    {
                        var checkListParent = await _context.FeedBackAddOn.Where(x => x.FeedBackId == checkParent.Id).ToListAsync();
                        foreach (var item in checkListParent)
                        {
                            var addQues = new FeedBackAddOn
                            {
                                Id = Guid.NewGuid(),
                                FeedBackId = fb.Id,
                                Question = item.Question,
                                Answer = null,
                            };
                            await _context.FeedBackAddOn.AddAsync(addQues);
                        }
                    }
                }
                await _context.FeedBack.AddAsync(fb);
                await _context.SaveChangesAsync();
                result.ResponseSuccess = fb;
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

        public async Task<ResultModel> DeleteQuestion(Guid Id, Guid questionId)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var op = _context.FeedBackAddOn.FirstOrDefault(x => x.FeedBackId == Id & x.Id == questionId);
                if (op == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = "question not found ";
                    return result;
                }
                _context.FeedBackAddOn.Remove(op);
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

        public async Task<ResultModel> GetChildFb(Guid parentId)
        {
            var result = new ResultModel();
            try
            {
                var root = _context.FeedBack.Include(x => x.Registration)
                                                    .ThenInclude(x => x.Student)
                                                    .Include(c => c.ChildrenFeedBacKs)
                                                    .Include(f=> f.FeedBackAddOns)
                            .Where(c => c.ParenFeedBacksId == parentId)
                                .AsParallel()
                                    .ToList();

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = root;

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetDetailByRes(Guid resId)
        {
            var result = new ResultModel();
            try
            {
                var res = _context.FeedBack.Where(x => x.RegistrationId == resId);

                if (res == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any feedback Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.FeedBack.Include(p => p.Registration).ThenInclude(s => s.Student).Include(x => x.FeedBackAddOns)
                                                                .Where(x => x.RegistrationId == resId).ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> GetDetailFbId(Guid Id)
        {
            var result = new ResultModel();
            try
            {
                var res = _context.FeedBack.Where(x => x.Id == Id);

                if (res == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any feedback Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.FeedBack.Include(p => p.Registration).ThenInclude(s => s.Student).Include(x => x.FeedBackAddOns)
                                                                .Where(x => x.Id == Id).ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> GetDetailFeedBackByStudentId(Guid studentId)
        {
            var result = new ResultModel();
            try
            {
                var res = _context.Registration.Where(x => x.StudentId == studentId);

                if (res == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any feedback Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Registration.Include(s => s.FeedBacks).ThenInclude(x => x.FeedBackAddOns)
                                                                .Where(x => x.StudentId == studentId).ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> GetRootFeedBack()
        {
            var result = new ResultModel();
            try
            {

                List<FeedBack> fb = await _context.FeedBack
                                                    .Include(x => x.Registration).ThenInclude(c => c.Student)
                                                    .Include(f => f.FeedBackAddOns)
                                                    .AsNoTrackingWithIdentityResolution()
                                                                .ToListAsync();
                List<FeedBack> root =  fb
                            .Where(c => c.ParenFeedBacksId == null)
                                .AsParallel()
                                    .ToList();

                result.IsSuccess = true;
                result.Code = 200;
                result.ResponseSuccess = root;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> UpdateAnswer(UpdateFeedBackAnswerModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var op = await _context.FeedBackAddOn.Where(x => x.FeedBackId == model.FeedbackId & x.Id == model.Id).FirstOrDefaultAsync();
                if (op == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = "Question not found ";
                    return result;
                }
                op.Answer = model.Answer;
                _context.FeedBackAddOn.Update(op);
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

        public async Task<ResultModel> UpdateFeedBackId(Guid Id, UpdateFeedbackInfoModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var op = await _context.FeedBack.Where(x => x.Id == Id).FirstOrDefaultAsync();
                if (op == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = "Form feedback not found ";
                    return result;
                }
                op.ParenFeedBacksId = model.ParentFeedBacksId;
                op.FeedBackContent = model.FeedBackContent;
                op.Description = model.Description;
                op.Title = model.Title;
                op.Status = model.Status;
                op.RegistrationId = model.RegistrationId;
                _context.FeedBack.Update(op);
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

        public async Task<ResultModel> UpdateQuestion(UpdateFeedBackQuestionModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var op = await _context.FeedBackAddOn.Where(x => x.FeedBackId == model.FeedbackId & x.Id == model.Id).FirstOrDefaultAsync();
                if (op == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = "Question not found ";
                    return result;
                }
                op.Question = model.Question;
                _context.FeedBackAddOn.Update(op);
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
    }
}

