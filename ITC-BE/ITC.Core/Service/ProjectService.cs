using System;
using System.Net.NetworkInformation;
using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using ITC.Core.Data;
using ITC.Core.Interface;
using ITC.Core.Model;
using ITC.Core.Utilities.Email;
using ITC.Data.Entities;
using ITC.Data.Enum;
using ITC.Data.Utilities.Paging;
using ITC.Data.Utilities.Paging.PaginationModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace ITC.Core.Service
{
    public class ProjectService : IProjectService
    {
        private readonly ITCDBContext _context;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        private readonly IUserContextService _userContextService;
        public ProjectService(ITCDBContext context, IMapper mapper, IEmailService emailService,
                            IConfiguration config, IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _emailService = emailService;
            _config = config;
            _userContextService = userContextService;
        }

        public async Task<ResultModel> AssignStaffIntoProject(Guid Id, List<Guid>? staffId)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var projects = await _context.Project.ToListAsync();
                var co = from n in _context.JoinProject
                         where n.ProjectId == Id
                         select n;
                int num = co.Count();
                if (num > 5)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = "project has 5 staff.........";
                    return result;
                }
                if (projects == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = " project null.........";
                    return result;
                }

                if (staffId != null)
                {
                    foreach (var item in staffId)
                    {
                        var joinProject = await _context.JoinProject.FirstOrDefaultAsync(x => x.ProjectId == Id && x.StaffId == item);

                        if (joinProject != null)
                        {
                            result.Code = 400;
                            result.IsSuccess = false;
                            result.ResponseSuccess = "staff is existed in Project.........";
                            return result;
                        }
                        var joinP = new JoinProject
                        {
                            Id = Guid.NewGuid(),
                            StaffId = item,
                            ProjectId = Id,
                            IsLeader = false,
                            AssignDate = DateTime.Now
                        };
                        await _context.JoinProject.AddAsync(joinP);
                    }
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
        public async Task<ResultModel> UnAssignStaffIntoProject(Guid projectId, Guid StaffId)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var staff = _context.JoinProject.FirstOrDefault(x => x.ProjectId == projectId & x.StaffId == StaffId);
                if (staff == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = "staff not found ";
                    return result;
                }
                _context.JoinProject.Remove(staff);
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

        public async Task<ResultModel> CreateProject(ProjectCreateModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var header = _context.Staff.FirstOrDefault(x => x.IsHeadOfDepartMent == true && _userContextService.UserID == x.AccountId);
                var admin = _context.Account.FirstOrDefault(x => x.AccountId == _userContextService.UserID && x.Role == RoleEnum.Admin);
                var staffInfo = _context.Staff.FirstOrDefault(x => x.Id == model.LeaderId);
                var getInfo = _context.Account.FirstOrDefault(a => a.AccountId == staffInfo.AccountId);
                var getNewestConfig = _context.ConfigProject.OrderByDescending(c => c.DateCreated).FirstOrDefault();
                var checkSyllabus = await _context.Syllabus.Where(x => x.CourseId == model.CourseId).ToListAsync();
                var CheckName = await _context.Project.Where(x => x.ProjectName == model.ProjectName).FirstOrDefaultAsync();
                var checkCourse = await _context.Project.Where(c => c.CourseId == model.CourseId && c.PartnerId == model.PartnerId).FirstOrDefaultAsync();
                if (header != null || admin != null)
                {

                    var co = await _context.JoinProject.Where(x => x.StaffId == model.LeaderId && x.IsLeader == true).ToListAsync();
                    int num = co.Count();
                    if (num >= 5)
                    {
                        result.Code = 400;
                        result.IsSuccess = false;
                        result.ResponseSuccess = "Leader has 5 project.........";
                        return result;
                    }

                    if (checkCourse != null)
                    {
                        result.Code = 400;
                        result.IsSuccess = false;
                        result.ResponseFailed = $"Course : {model.CourseId}  and Partner : {model.PartnerId} existed!!";
                        return result;
                    };
                
                    if (CheckName != null)
                    {
                        result.Code = 400;
                        result.IsSuccess = false;
                        result.ResponseFailed = $"ProjectName : {model.ProjectName}  existed!!";
                        return result;
                    };
                    var project = new Project
                    {
                        Id = Guid.NewGuid(),
                        ProjectName = model.ProjectName,
                        CampusName = model.CampusName,
                        EstimateTimeStart = model.EstimateTimeStart,
                        EstimateTimeEnd = model.EstimateTimeEnd,
                        Description = model.Description,
                        OfficalTimeStart = model.OfficalTimeStart,
                        OfficalTimeEnd = model.OfficalTimeEnd,
                        DateCreated = DateTime.Now,
                        ProjectStatus = ProjectStatusEnum.Active,
                        Creater = _context.Account.Where(x => x.AccountId == _userContextService.UserID).FirstOrDefault(),
                        CourseId = model.CourseId,
                        PartnerId = model.PartnerId,
                        LeaderId = model.LeaderId,
                        ProgramId = model.ProgramId,
                        CampusId = model.CampusId,
                        ConfigProjectId = getNewestConfig?.Id,
                        CheckNegotiationStatus = false
                    };

                    foreach (var item in checkSyllabus)
                    {
                        if (item.PartnerId != model.PartnerId && item.CourseId != model.CourseId)
                        {
                            var syll = new Syllabus
                            {
                                Id = Guid.NewGuid(),
                                Content = item.Content,
                                Description = item.Description,
                                DateCreated = DateTime.Now,
                                PartnerId = model.PartnerId,
                                CourseId = model.CourseId,
                                Status = false
                            };
                            await _context.Syllabus.AddAsync(syll);
                        }
                    }
                    if (model.PhaseId != null)
                    {
                        foreach (var item in model.PhaseId)
                        {
                            ProjectPhase mp = new ProjectPhase
                            {
                                Id = Guid.NewGuid(),
                                ProjectId = project.Id,
                                PhaseId = item,
                                DateBegin = null,
                                DateEnd = null
                            };
                            await _context.ProjectPhase.AddAsync(mp);
                        }
                    }
                    var join = new JoinProject
                    {
                        Id = Guid.NewGuid(),
                        StaffId = model.LeaderId,
                        ProjectId = project.Id,
                        IsLeader = true,
                        AssignDate = DateTime.Now,
                    };
                    _emailService.SendAssign(_config["Emails:SmtpUser"], getInfo.Email, getInfo.FullName);
                    await _context.Project.AddAsync(project);
                    await _context.JoinProject.AddAsync(join);
                    await _context.SaveChangesAsync();

                    result.IsSuccess = true;
                    result.Code = 200;
                    result.ResponseSuccess = await _context.Project.Include(p => p.Partner).AsNoTrackingWithIdentityResolution()
                                                                    .Include(c => c.Course)
                                                                    .Include(j => j.JoinProjects)
                                                                    .ThenInclude(s => s.Staffs)
                                                                    .Include(emp => emp.Tasks)
                                                                    .Include(mile => mile.ProjectPhase).ThenInclude(ml => ml.Phase)
                                                                    .Where(x => x.Id == project.Id).ToListAsync();

                    await transaction.CommitAsync();
                }
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = "Project must be created by head of IC or admin!";
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

        public async Task<ResultModel> DeleteProject(Guid id)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var project = await _context.Project.FindAsync(id);
                if (project == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = $"Prj with id: {id} not existed!!";
                    return result;
                }

                project.ProjectStatus = ProjectStatusEnum.InActive;
                _context.Project.Update(project);
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

        public async Task<ResultModel> GetAllProject()
        {
            var result = new ResultModel();
            try
            {
                var project = _context.Project.AsNoTrackingWithIdentityResolution()
                    .Include(x => x.Creater)
                    .Include(p => p.Partner)
                    .Include(c => c.Course)
                    .Include(c => c.Program)
                    .Include(c => c.Campus)
                    .Include(mile => mile.ProjectPhase)
                    .ThenInclude(ml => ml.Phase)
                    .Include(emp => emp.Tasks.OrderBy(x => x.DeadLine));
                //.Include(s => s.Program);
                //.ToListAsync();

                if (project == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Project Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = _mapper.ProjectTo<ProjecDTOModel>(project).ToList();
                //result.ResponseSuccess = await _context.Project.AsNoTrackingWithIdentityResolution().Include(x => x.Creater)
                //    .Include(mile => mile.ProjectPhase)
                //    .ThenInclude(ml => ml.Phase)
                //    .Include(emp => emp.Tasks.OrderBy(x => x.DeadLine))
                //    .Include(s => s.Program)
                //    .ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetDetailProject(Guid Id)
        {
            var result = new ResultModel();
            try
            {
                var prj = _context.Project.Where(x => x.Id == Id);

                if (prj == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Project Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Project.Where(x => x.Id == Id).AsNoTrackingWithIdentityResolution()
                                                        .Include(j => j.JoinProjects).ThenInclude(x => x.Staffs)
                                                        .Include(p => p.Partner)
                                                        .Include(c => c.Course)
                                                        .Include(emp => emp.Tasks.OrderBy(x => x.DeadLine))
                                                        .Include(cmp => cmp.Campus)
                                                        .Include(cmp => cmp.Program)
                                                        .Include(cmp => cmp.CancelProjects)
                                                        .ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> GetJoinProject()
        {
            var result = new ResultModel();
            try
            {
                var jp = _context.JoinProject;



                if (jp == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Project Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.JoinProject.AsNoTrackingWithIdentityResolution()
                                                        .Include(x => x.Staffs)
                                                        .ToListAsync();
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }


        public async Task<ResultModel> GetJoinProjectId(Guid projectId)
        {
            var result = new ResultModel();
            try
            {
                var jp = _context.JoinProject.Where(x => x.ProjectId == projectId);



                if (jp == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Project Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.JoinProject.AsNoTrackingWithIdentityResolution().Where(x => x.ProjectId == projectId)
                                                        .Include(x => x.Staffs)
                                                        .ToListAsync();
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> SearchProject(string keyword)
        {
            var result = new ResultModel();
            try
            {
                var projects = await _context.Project.Include(x => x.Course).Where(x => x.ProjectName!.Contains(keyword)).ToListAsync();
                if (!projects.Any())
                {
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.ResponseSuccess = new ProjecViewModel();
                    return result;
                }

                result.IsSuccess = true;
                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = _mapper.Map<List<ProjecViewModel>>(projects);

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> UpdateProject(Guid Id, ProjectUpdateModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var project = await _context.Project.FindAsync(Id);
                var co = await _context.JoinProject.Where(x => x.StaffId == model.LeaderId && x.IsLeader == true && x.ProjectId != Id).ToListAsync();
                var co1 = await _context.JoinProject.Where(x => x.StaffId == project.LeaderId && x.ProjectId == Id && x.IsLeader == true).FirstOrDefaultAsync();
                var CheckName = await _context.Project.Where(x => x.ProjectName == model.ProjectName).FirstOrDefaultAsync();
                int num = co.Count();
                if (num >= 5)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = "Leader has 5 project.........";
                    return result;
                }
                if (project == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = "$ can't found Project {Id}";
                    return result;
                }

                if (model.ProjectName != null)
                {
                    project.ProjectName = model.ProjectName;
                }
                project.EstimateTimeStart = model.EstimateTimeStart;
                project.EstimateTimeEnd = model.EstimateTimeEnd;
                project.Description = model.Description;
                project.OfficalTimeStart = model.OfficalTimeStart;
                project.OfficalTimeEnd = model.OfficalTimeEnd;
                project.ProjectStatus = model.ProjectStatus!.Value;
                project.CheckNegotiationStatus = model.CheckNegotiationStatus;
                project.CourseId = model.CourseId;
                project.PartnerId = model.PartnerId;
                project.ProgramId = model.ProgramId;
                project.CampusId = model.CampusId;
                project.ConfigProjectId = model.ConfigId;

                foreach (var item in co)
                {
                    if (co1 != null)
                    {
                        co1.StaffId = model.LeaderId;
                        co1.IsLeader = true;
                        co1.AssignDate = DateTime.Now;
                        _context.JoinProject.Update(co1);
                    }

                }

                if (co1 != null)
                {
                    project.LeaderId = model.LeaderId;
                }

                _context.Project.Update(project);
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


        public async Task<ResultModel> ChangeStatusProject(ChangeStatusProject model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var project = _context.Project.FirstOrDefault(x => x.Id == model.ProjectId);
                if (project == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = "Can't found project";
                    return result;
                }

                project.ProjectStatus = model.Status;
                if (project.ProjectStatus == ProjectStatusEnum.Canceled)
                {
                    var rm = await _context.JoinProject.Where(x => x.ProjectId == model.ProjectId).ToListAsync();
                    foreach (var item in rm)
                    {
                        _context.JoinProject.Remove(item);
                        await _context.SaveChangesAsync();
                    }
                    project.LeaderId = Guid.Empty;
                    _context.Project.Update(project);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }

                _context.Project.Update(project);
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

