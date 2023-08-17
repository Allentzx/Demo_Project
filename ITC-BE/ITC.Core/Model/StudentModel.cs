using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace ITC.Core.Model
{
    public class StudentModel
    {
    }
    public class StudentAuthenModel
    {
        public Guid? Id { get; set; }
        public string? RollNumber { get; set; }
        public string? MemberCode { get; set; }
        public string? FullName { get; set; }
        public string? OldRollNumber { get; set; }
        public string? MajorName { get; set; }
        public string? Batch { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public bool Status { get; set; }
        public Guid? MajorId { get; set; }
        public string? TokenToken { get; set; }
    }
    public class StudentViewModel
    {
        public string? RollNumber { get; set; }
        public string? MemberCode { get; set; }
        public string? FullName { get; set; }
        public string? OldRollNumber { get; set; }
        public string? MajorName { get; set; }
        public string? Batch { get; set; }
        public string? UpStatus { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public bool Status { get; set; }

    }

    public class CreateStudentModel
    {
        public string? RollNumber { get; set; }
        public string? MemberCode { get; set; }
        public string? FullName { get; set; }
        public string? OldRollNumber { get; set; }
        public string? MajorName { get; set; }
        public string? Batch { get; set; }
        public string? Semeter { get; set; }
        public string? UpStatus { get; set; }
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@(fpt.edu.vn)$", ErrorMessage = "Invalid edu Email address.")]
        public string? Email { get; set; }
        [Phone]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Provided phone number not valid")]
        [StringLength(10, MinimumLength = 10)]
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? GradingUrl { get; set; }
        public bool Status { get; set; }

        public Guid? MajorId { get; set; }
    }

    public class UpdateStudentModel
    {
        public string? RollNumber { get; set; }
        public string? MemberCode { get; set; }
        public string? FullName { get; set; }
        public string? OldRollNumber { get; set; }
        //public string? MajorName { get; set; }
        public string? Batch { get; set; }
        public string? Semeter { get; set; }
        public string? UpStatus { get; set; }
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@(fpt.edu.vn)$", ErrorMessage = "Invalid edu Email address.")]
        public string? Email { get; set; }
        [DataType(DataType.PhoneNumber, ErrorMessage = "Provided phone number not valid")]
        [StringLength(10, MinimumLength = 10)]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Phone Number.")]
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public bool Status { get; set; }

        public Guid? MajorId { get; set; }
        public Guid? RegistrationId { get; set; }

    }

    public class ImportStudentModel
    {
        public IFormFile? formFile { get; set; }
        //public Guid? MajorId { get; set; }
    }

    public class ViewRegistrationProjectModel
    {
        public Guid Id { get; set; }

        public string? OldPassportImageUrl { get; set; }
        public string? NewPassportImageUrl { get; set; }
        public string? UrlImageBill { get; set; }
        public bool Status { get; set; }
        public DateTime? DateOpenRegis { get; set; }
        public DateTime? DateCloseRegis { get; set; }
        // FK 
        public Guid StudentId { get; set; }
        public Guid ProjectId { get; set; }
    }
        public class RegistrationProjectModel
    {
        public string? RollNumber { get; set; }
        public string? MemberCode { get; set; }
        public string? FullName { get; set; }
        public string? OldRollNumber { get; set; }
        public string? MajorName { get; set; }
        public string? Batch { get; set; }
        public string? Semeter { get; set; }
        public string? UpStatus { get; set; }
        public string? Email { get; set; }
        [Phone]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Provided phone number not valid")]
        [StringLength(10, MinimumLength = 10)]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Phone Number.")]
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public bool Status { get; set; }

        public FormFile? GradingFile { get; set; }
        public FormFile? PassportFile { get; set; }
        // FK 
        public Guid StudentId { get; set; }
        public Guid ProjectId { get; set; }
        //public Guid? MajorId { get; set; }
        //public Guid? RegistrationId { get; set; }
    }

    public class UploadResponse
    {
        public string? Status { get; set; }
        public bool Error { get; set; }
        public string? Uri { get; set; }
        public string? Name { get; set; }
        public string? ContentType { get; set; }
        public Stream? Content { get; set; }
    }
}

