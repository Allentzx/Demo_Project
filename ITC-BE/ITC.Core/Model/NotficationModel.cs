using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITC.Core.Model
{
    public class NotficationModel
    {

    }

    public class NotificationRespone
    {
        public Guid Id { get; set; }
        public DateTime DateCreate { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
        public bool Read { get; set; }
        //sender and content
        public Guid? UserSendId { get; set; }
        public string? BodyCustom { get; set; }
        public string? FullName { get; set; }
        //User
        public Guid? AccountId { get; set; }
        //TaskComment
        public Guid? TaskCommnetId { get; set; }
        //Project
        public Guid? ProjectId { get; set; }
        //Task
        public Guid? TasksId { get; set; }
    }


    public class HistoryRespone
    {
        public Guid Id { get; set; }
        public DateTime DateCreate { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
        public bool Read { get; set; }
        //sender and content
        public Guid? UserSendId { get; set; }
        public string? FullName { get; set; }
        public string? BodyCustom { get; set; }
        //User
        public Guid? AccountId { get; set; }
        //TaskComment
        public Guid? TaskCommnetId { get; set; }
        //Project
        public Guid? ProjectId { get; set; }
        //Task
        public Guid? TasksId { get; set; }
    }
}

