using System.ComponentModel.DataAnnotations;

namespace ITC.Data.Entities
{
    public class JoinProject
    {
        [Key]
        public Guid Id { get; set; }
        public bool IsLeader { get; set; }
        public DateTime AssignDate { get; set; }


        //FK
        public Guid ProjectId { get; set; }
        public Guid? StaffId { get; set; }

        public virtual Project? Project { get; set; }
        public virtual Staff? Staffs { get; set; }
    }
}
