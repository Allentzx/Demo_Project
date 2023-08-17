using AutoMapper;
using ITC.Core.Model;
using ITC.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Core.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Nullable type
            CreateMap<int?, int>().ConvertUsing((src, des) => src ?? des);
            CreateMap<bool?, bool>().ConvertUsing((src, des) => src ?? des);
            CreateMap<Guid?, Guid>().ConvertUsing((src, des) => src ?? des);
            CreateMap<DateTime?, DateTime>().ConvertUsing((src, des) => src ?? des);
            #endregion
            #region Staff
            CreateMap<Staff, StaffViewModel>();

            #endregion
            #region Deputy
            CreateMap<Deputy, DeputyViewModel>();
            CreateMap<Deputy, ViewNameDeputitesModel>();

            #endregion
            #region Post
            CreateMap<Post, PostViewModel>();

            #endregion

            #region Partner
            CreateMap<Partner, PartnerViewModel>();

            #endregion
            #region Course
            CreateMap<Course, CourseViewModel>();
            CreateMap<Course, ViewCourseModel>();
            CreateMap<Course, ViewCourseDetailModel>();
            #endregion

            #region Syllabus
            CreateMap<Syllabus, SyllabusViewModel>();
            CreateMap<Syllabus, SyllabusViewCourseModel>();
            #endregion

            #region Slot
            CreateMap<Slot, SlotViewModel>();
            CreateMap<Slot, SlotModel>();
            CreateMap<Slot, ViewSlotModel>();


            #endregion
            #region Reason
            CreateMap<Reason, ReasonViewModel>();


            #endregion

            #region Task
            CreateMap<Tasks, TaskViewModel>();
            CreateMap<Tasks, TaskModel>();
            #endregion

            #region Phase
            CreateMap<ProjectPhase, ProjectPhaseModel>();
            CreateMap<Phase, PhaseViewModel>();
            #endregion

            #region Program
            CreateMap<Program, ProgramModel>();
            #endregion
            #region Student
            CreateMap<Student, StudentViewModel>();

            #endregion


            #region Account
            CreateMap<Account, ViewCurrentAccountModel>();
            CreateMap<Account, AccountModel>();
            #endregion

            #region Project
            CreateMap<Project, ProjecViewModel>();
            CreateMap<Project, ProjecDTOModel>();

            #endregion
            #region Campus
            CreateMap<Campus, CampusViewModel>();

            #endregion
            #region Registration
            CreateMap<Registration, RegistrationProjectModel>();
            CreateMap<Registration, ViewRegistrationProjectModel>();

            #endregion
            #region Role
            //CreateMap<Role, RoleViewModel>();
            //CreateMap<RoleCreateModel, Role>();
            //CreateMap<RoleUpdateModel, Role>();
            #endregion
        }
    }
}
