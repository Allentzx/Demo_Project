using AutoMapper;
using ITC.Core.Data;
using ITC.Core.Interface;
using ITC.Core.Model;
using ITC.Data.Entities;
using ITC.Data.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Core.Service
{
    public class TaskService : ITaskService
    {
        private readonly ITCDBContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;
        public TaskService(ITCDBContext context, IMapper mapper, IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task<ResultModel> AssignTask(Guid TasksId, Guid StaffId)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var task = await _context.AssignTask.ToListAsync();
                var Task = await _context.AssignTask.FirstOrDefaultAsync(x => x.StaffId == StaffId && x.TasksId == TasksId);
                if (task == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = "Task null.........";
                    return result;
                }
                if (Task != null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = "staff is existed in task.........";
                    return result;
                }
                var empInTask = new AssignTask
                {
                    Id = Guid.NewGuid(),
                    StaffId = StaffId,
                    TasksId = TasksId,
                    AssignDate = DateTime.Now
                };
                await _context.AssignTask.AddAsync(empInTask);
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


        public async Task<ResultModel> UnAssignTask(Guid Id, Guid StaffId)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var task = _context.AssignTask.FirstOrDefault(x => x.TasksId == Id & x.StaffId == StaffId);
                if (task == null)
                {
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.ResponseSuccess = "task null";
                    return result;
                }
                _context.AssignTask.Remove(task);
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

        public async Task<ResultModel> CreateTask(TaskCreateModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var tasks = _context.Tasks;
                var header = _context.Staff.FirstOrDefault(x => x.IsHeadOfDepartMent == true && _userContextService.UserID == x.AccountId);
                var staff = _context.Staff.FirstOrDefault(x => _userContextService.UserID == x.AccountId);
                var checkLeader = await _context.JoinProject.Where(x => x.ProjectId == model.ProjectId && x.IsLeader == true && x.StaffId == staff!.Id).FirstOrDefaultAsync();
                if (tasks == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any tasks Not Found!";
                    return result;
                }
                if (header != null || checkLeader != null)
                {
                    var task = new Tasks
                    {
                        Id = Guid.NewGuid(),
                        ParentId = model.ParentTaskId,
                        TaskName = model.TaskName,
                        DeadLine = model.DeadLine,
                        Description = model.Description,
                        State = TaskStateEnum.Todo,
                        Status = TaskStatusEnum.Todo,
                        Creater = _userContextService.FullName,
                        DateCreated = DateTime.Now,
                        ProjectId = model.ProjectId,
                        PhaseId = model.PhaseId
                    };
                    if (model.StaffId != null)
                    {
                        foreach (var item in model.StaffId)
                        {
                            AssignTask empInTask = new AssignTask()
                            {

                                Id = Guid.NewGuid(),
                                StaffId = item,
                                TasksId = task.Id,
                                AssignDate = DateTime.Now
                            };
                            await _context.AssignTask.AddAsync(empInTask);
                        }
                    }
                    await _context.Tasks.AddAsync(task);
                    await _context.SaveChangesAsync();

                    result.IsSuccess = true;
                    result.Code = 200;

                    await transaction.CommitAsync();
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.ResponseSuccess = "Task must be create by HOD IC or leader of project";
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

        public async Task<ResultModel> DisableTask(Guid id)
        {

            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var task = await _context.Tasks.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (task == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = $"task with id: {id} not existed!!";
                    return result;
                }
                task.State = TaskStateEnum.DeActive;
                task.Status = TaskStatusEnum.DeActive;
                _context.Tasks.Update(task);
                await _context.SaveChangesAsync();

                result.Code = 200;
                result.ResponseSuccess = "Succesfull";
                result.IsSuccess = true;
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            //await transaction.CommitAsync();
            return result;
        }



        public async Task<ResultModel> GetRootTask()
        {
            var result = new ResultModel();
            try
            {

                List<Tasks> task = await _context.Tasks
                                                    .AsNoTrackingWithIdentityResolution().OrderByDescending(x => x.DateCreated)
                                                        .Include(c => c.ChildrenTask)
                                                        .Include(x => x.AssignTasks)
                                                                //.ThenInclude(x => x.Staffs)
                                                                //.ThenInclude(x => x.Account)
                                                                .ToListAsync();
                // Structure Task into a tree
                List<Tasks> rootTask = task
                            .Where(c => c.ParentId == null)
                                .AsParallel()
                                    .ToList();

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = rootTask;

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetChildTask(Guid parentId)
        {
            var result = new ResultModel();
            try
            {
                var rootTask = _context.Tasks
                                                                .Include(x => x.AssignTasks)
                                                                .ThenInclude(x => x.Staffs)
                                                                .ThenInclude(x => x.Account)
                            .Where(c => c.ParentId == parentId)
                                .AsParallel()
                                    .ToList();

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = rootTask;

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }


        public async Task<ResultModel> GetDetailTask(Guid Id)
        {
            var result = new ResultModel();
            try
            {
                var task = _context.Tasks.Where(x => x.Id == Id);

                if (task == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Task Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Tasks.Include(p => p.Project).Include(f => f.ProjectPhase.Phase)
                                                                .Include(x => x.AssignTasks).ThenInclude(s => s.Staffs).ThenInclude(x => x.Account)
                                                                .Where(x => x.Id == Id).ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

         public async Task<ResultModel> GetAllTask()
        {
            var result = new ResultModel();
            try
            {
                var task = await _context.Tasks.ToListAsync();

                if (task == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Task Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = task;

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }
        public async Task<ResultModel> UpdateTask(Guid Id, TaskUpdateModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var task = await _context.Tasks.FindAsync(Id);
                if (task == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = "Can't found project ${Id}";
                    return result;
                }

                task.TaskName = model.TaskName;
                task.DeadLine = model.DeadLine;
                task.Description = model.Description;
                task.State = model.State!.Value;
                task.Status = model.Status!.Value;
                task.ProjectId = model.ProjectId;
                task.PhaseId = model.PhaseId;
                task.DateCreated = model.DateCreate;

                _context.Tasks.Update(task);
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



        public async Task<ResultModel> ChangeStatusTask(ChangeStatus model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var task = _context.Tasks.FirstOrDefault(x => x.Id == model.TaskId);
                if (task == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = "Can't found Staff in task";
                    return result;
                }

                task.Status = model.TaskStatus;

                _context.Tasks.Update(task);
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


        public async Task<ResultModel> ChangeStateTask(ChangeState model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var task = _context.Tasks.FirstOrDefault(x => x.Id == model.TaskId);
                if (task == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = "Can't found Staff in task";
                    return result;
                }

                task.State = model.State;

                _context.Tasks.Update(task);
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

        public async Task<ResultModel> SearchTask(string keyword)
        {
            var result = new ResultModel();
            try
            {
                var task = await _context.Tasks.Where(x => x.TaskName.Contains(keyword)).ToListAsync();
                if (!task.Any())
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = "not found";
                    return result;
                }

                result.IsSuccess = true;
                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = task;

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetTaskByProjectId(Guid projectId)
        {
            var result = new ResultModel();
            try
            {
                var task = await _context.Tasks.Where(x => x.ProjectId== projectId).Include(x=> x.ProjectPhase).ToListAsync();
                if (!task.Any())
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = "not found";
                    return result;
                }

                result.IsSuccess = true;
                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = task;

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
