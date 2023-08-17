using System;
using ITC.Data.Entities;

namespace ITC.Core.Model
{
    public class CourseModel
    {
        public string? Activity { get; set; }
        public string? Content { get; set; }
        public string? CourseName { get; set; }
        public DateTime? DateCreated { get; set; }
        public AccountModel? Creater { get; set; }
        public bool Status { get; set; }
    }

    public class CourseViewModel : CourseModel
    {
        public Guid Id { get; set; }

        public List<SyllabusViewModel>? Syllabus { get; set; }
    }


    public class ViewCourseModel
    {
        public Guid Id { get; set; }
        public string? Activity { get; set; }
        public string? Content { get; set; }
        public string? CourseName { get; set; }
        public DateTime? DateCreated { get; set; }
        public bool Status { get; set; }
        public ViewCurrentAccountModel? Creater { get; set; }

    }
    public class ViewCourseDetailModel
    {
        public Guid Id { get; set; }
        public string? CourseName { get; set; }
        public string? Activity { get; set; }
        public string? Content { get; set; }
        public DateTime? DateCreated { get; set; }
        public Account? Creator { get; set; }
        public bool Status { get; set; }

        public ICollection<SyllabusViewCourseModel>? Syllabus { get; set; }

    }



   

    public class CreateCourseModel
    {
        public string? Activity { get; set; }
        public string? Content { get; set; }
        public string? CourseName { get; set; }
    }

    public class UpdateCourseModel
    {
        public string? Activity { get; set; }
        public string? Content { get; set; }
        public string? CourseName { get; set; }
        public DateTime? DateCreate { get; set; }
        public bool Status { get; set; }
    }

   
}

