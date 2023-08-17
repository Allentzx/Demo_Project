using System;
using ITC.Data.Entities;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ITC.Core.Model
{
    public class RegistrationModel
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public string? Title { get; set; }
        public string? NumberPassPort { get; set; }
        public string? PassportImageUrl { get; set; }
        public string? UrlImageBill { get; set; }
        public string? ScocialLink { get; set; }
        public string? YourEmail { get; set; }
        public string? Creator { get; set; }
        public bool Status { get; set; }
        public DateTime? DateExpired { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateOpenRegis { get; set; }
        public DateTime? DateCloseRegis { get; set; }

        public virtual Registration? Registrations { get; set; }
        public ICollection<Registration>? ChildrenRegistrations { get; set; }

        // FK 
        public Guid? StudentId { get; set; }
        public Guid? ProjectId { get; set; }
    }


    public class CreateRegistrationModel
    {
        public Guid? ParentId { get; set; }
        public string? Title { get; set; }
        public string? NumberPassPort { get; set; }
        public string? ScocialLink { get; set; }
        public string? YourEmail { get; set; }
        public DateTime? DateExpired { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateOpenRegis { get; set; }
        public DateTime? DateCloseRegis { get; set; }
        public List<string>? AddMoreOptinal { get; set; }
        // FK 
        public Guid? StudentId { get; set; }
        public Guid? ProjectId { get; set; }
    }

    public class CreateOptionalModel
    {
        public Guid? RegistrationsId { get; set; }
        public List<string>? Question { get; set; }
    }


    public class UpdateRegisInfoModel
    {
        public string? Title { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? NumberPassPort { get; set; }
        public string? RollNumber { get; set; }
        public string? ScocialLink { get; set; }
        public string? YourEmail { get; set; }
        public IFormFile? PassportImageUrl { get; set; }
        public IFormFile? UrlImageBill { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateExpired { get; set; }
        public string? Content { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? MajorId { get; set; }
    }
    public class UpdateRegisModel
    {
        public string? Title { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? NumberPassPort { get; set; }
        public string? RollNumber { get; set; }
        public string? ScocialLink { get; set; }
        public string? YourEmail { get; set; }
        public IFormFile? PassportImageUrl { get; set; }
        public IFormFile? UrlImageBill { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateExpired { get; set; }
        public DateTime? DateOpenRegis { get; set; }
        public DateTime? DateCloseRegis { get; set; }
        public string? ContentHeader { get; set; }

        // FK 
        public Guid? ProjectId { get; set; }
        public Guid? MajorId { get; set; }
    }

    public class UpdateQuestionModel
    {
        [Required]
        public Guid? RegistrationId { get; set; }
        [Required]
        public Guid? Id { get; set; }
        public string? Question { get; set; }

    }

     public class UpdateAnswerModel
    {
        [Required]
        public Guid? RegistrationId { get; set; }
        [Required]
        public Guid? Id { get; set; }
        public string? Answer { get; set; }

    }

}

