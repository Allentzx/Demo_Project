using System;
using ITC.Data.Enum;

namespace ITC.Data.Entities
{
    public class AssignTask
    {
        public Guid? Id { get; set; }

        //FK
        public Guid? TasksId { get; set; }
        public Guid? StaffId { get; set; }
        public DateTime AssignDate { get; set; }
        public virtual Tasks? Tasks { get; set; }
        public virtual Staff? Staffs { get; set; }
    }
}

