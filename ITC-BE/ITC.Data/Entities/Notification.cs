using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITC.Data.Entities
{
    public class Notification
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime DateCreate { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string? Title { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string? Body { get; set; }
        public bool Read { get; set; }


        //sender and content
        public Guid? UserSendId { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string? BodyCustom { get; set; }
        public string? FullName { get; set; }
        //User
        public Guid? AccountId { get; set; }
        //TaskComment
        public Guid? TaskCommnetId { get; set; }
        //Project
        public Guid? ProjectId {get; set;}
        //Task
        public Guid? TasksId { get; set; }


        //Account
        public Account? Account { get; set; }
    }
}
