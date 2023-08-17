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
    public class PhaseService : IPhaseService
    {
        private readonly ITCDBContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;
        public PhaseService(ITCDBContext context, IMapper mapper, IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task<ResultModel> CreatePhase(PhaseCreateModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var checkName = await _context.Phase.Where(x => x.PhaseName == model.PhaseName).FirstOrDefaultAsync();
                if (checkName != null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = $"Phase with  {checkName.PhaseName} is existed!!";
                    return result;
                }

                var fp = new Phase
                {
                    PhaseName = model.PhaseName,
                    Status = false,
                    DateCreate = DateTime.Now,
                };

                await _context.Phase.AddAsync(fp);
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

        public async Task<ResultModel> DeletePhase(int id)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var f = await _context.Phase.FirstOrDefaultAsync(x => x.Id == id);

                if (f == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = $"Phase with id: {id} not existed!!";
                    return result;
                }

                f.Status = false;
                _context.Phase.Update(f);
                await _context.SaveChangesAsync();

                result.Code = 200;
                result.ResponseSuccess = "Phase is disable";
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



        public async Task<ResultModel> GetAllPhase()
        {
            var result = new ResultModel();
            try
            {
                var f = _context.Phase;

                if (f == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any phase Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Phase.ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> GetPhaseByProjectId(Guid? projectId)
        {
            var result = new ResultModel();
            try
            {
                var f = await _context.ProjectPhase.Where(x => x.ProjectId == projectId).Include(x => x.Phase).ToListAsync();

                if (f == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any phase Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = f;

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> GetPhaseById(int Id)
        {
            var result = new ResultModel();
            try
            {
                var fp = _context.Phase.Where(x => x.Id == Id);

                if (fp == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any phase Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Phase.Where(x => x.Id == Id).ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> UpdateDatePhase(UpdateDatePhase model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {

                var Phase = _context.ProjectPhase.FirstOrDefault(x => x.ProjectId == model.ProjectId && x.PhaseId == model.PhaseId);
                var header = _context.Staff.FirstOrDefault(x => x.IsHeadOfDepartMent == true && _userContextService.UserID == x.AccountId);
                var admin = _context.Account.FirstOrDefault(x => x.AccountId == _userContextService.UserID && x.Role == RoleEnum.Admin);
                if (admin != null || header != null)
                {
                    if (Phase == null)
                    {
                        result.Code = 400;
                        result.IsSuccess = false;
                        result.ResponseSuccess = $"Any phase Not Found!";
                        return result;
                    }

                    Phase.PhaseId = model.PhaseId;
                    Phase.ProjectId = model.ProjectId;
                    Phase.DateBegin = model.DateBegin;
                    Phase.DateEnd = model.DateEnd;
                    Phase.Status = true;
                    _context.ProjectPhase.Update(Phase);
                    await _context.SaveChangesAsync();
                    result.IsSuccess = true;
                    result.Code = 200;
                    await transaction.CommitAsync();
                }
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = "Project must be Update by head of IC or admin!";
                    return result;
                }
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> UpdateLPhase(int Id, PhaseUpdateModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var fp = await _context.Phase.FindAsync(Id);
                if (fp == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any phase Not Found!";
                    return result;
                }

                fp.PhaseName = model.PhaseName;
                fp.Status = model.Status;
                _context.Phase.Update(fp);
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


        public async Task<ResultModel> UpdateStatusPhase(PhaseUpdateStatusModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var fp = await _context.ProjectPhase.Where(x => x.ProjectId == model.ProjectId && x.PhaseId == model.PhaseId).FirstOrDefaultAsync(); ;
                if (fp == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any phase Not Found!";
                    return result;
                }
                fp.Status = model.Status;
                _context.ProjectPhase.Update(fp);
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


        public async Task<ResultModel> AddPhaseIntoProject(Guid projectId, AssignPhase model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var projects = await _context.Project.Where(x => x.Id == projectId).FirstOrDefaultAsync();
                if (projects == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = " project null.........";
                    return result;
                }

                if (model.PhaseName != null)
                {
                    var checkName = await _context.Phase.Where(x => x.PhaseName == model.PhaseName).FirstOrDefaultAsync();
                    if (checkName != null)
                    {
                        result.Code = 400;
                        result.IsSuccess = false;
                        result.ResponseFailed = $"Phase with  {checkName.PhaseName} is existed!!";
                        return result;
                    }
                    var newPhase = new Phase
                    {
                        PhaseName = model.PhaseName,
                        Status = false,
                        DateCreate = DateTime.Now
                    };
                    await _context.Phase.AddAsync(newPhase);
                    await _context.SaveChangesAsync();

                    var newProjectPhase = new ProjectPhase
                    {
                        Id = Guid.NewGuid(),
                        PhaseId = newPhase.Id,
                        ProjectId = projectId,
                        DateBegin = model.DateBegin,
                        DateEnd = model.DateEnd,
                        Status = false
                    };
                    await _context.ProjectPhase.AddAsync(newProjectPhase);

                }
                if (model.PhaseId != null)
                {
                    var phase = await _context.ProjectPhase.FirstOrDefaultAsync(x => x.ProjectId == projectId && x.PhaseId == model.PhaseId);

                    if (phase != null)
                    {
                        result.Code = 400;
                        result.IsSuccess = false;
                        result.ResponseSuccess = "phase is existed in Project.........";
                        return result;
                    }
                    var pp = new ProjectPhase
                    {
                        Id = Guid.NewGuid(),
                        PhaseId = model.PhaseId,
                        ProjectId = projectId,
                        DateBegin = model.DateBegin,
                        DateEnd = model.DateEnd,
                        Status = false
                    };
                    await _context.ProjectPhase.AddAsync(pp);
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
        public async Task<ResultModel> RemovePhaseInProject(Guid projectId, int phaseId)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var phase = _context.ProjectPhase.FirstOrDefault(x => x.ProjectId == projectId & x.PhaseId == phaseId);
                if (phase == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = "phase not found ";
                    return result;
                }
                _context.ProjectPhase.Remove(phase);
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
