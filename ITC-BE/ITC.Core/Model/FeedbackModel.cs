using System;
using System.ComponentModel.DataAnnotations;
using ITC.Data.Entities;
using Microsoft.AspNetCore.Http;

namespace ITC.Core.Model
{
    public class FeedbackModel
    {

    }

    public class CreateFeedbackModel
    {
        public Guid? ParentFeedBacksId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? FeedBackContent { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Status { get; set; }
        public List<string>? AddMoreQuestion { get; set; }
        public Guid? RegistrationId { get; set; }
    }

    public class CreateFeedbacKQuestionModel
    {
        public Guid? FeedBackId { get; set; }
        public List<string>? Question { get; set; }
    }


    public class UpdateFeedbackInfoModel
    {
        public Guid? ParentFeedBacksId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? FeedBackContent { get; set; }
        public bool Status { get; set; }
        public Guid? RegistrationId { get; set; }
    }

    public class UpdateFeedBackQuestionModel
    {
        [Required]
        public Guid? FeedbackId { get; set; }
        [Required]
        public Guid? Id { get; set; }
        public string? Question { get; set; }

    }

    public class UpdateFeedBackAnswerModel
    {
        [Required]
        public Guid? FeedbackId { get; set; }
        [Required]
        public Guid? Id { get; set; }
        public string? Answer { get; set; }

    }
}

