using ITC.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace ITC.Data.Entities
{
    public class Staff
	{
        [Key]		
        public Guid Id { get; set; }
        public string? StaffCode { get; set; }
        public bool IsHeadOfDepartMent { get; set; }
        public Guid? AccountId { get; set; }
        public Account? Account { get; set; }

        public virtual ICollection<JoinProject>? JoinProjects { get; set; }
        public virtual ICollection<Post>? Posts { get; set; }
        public ICollection<AssignTask>? AssignTasks { get; set; }


    }
}

