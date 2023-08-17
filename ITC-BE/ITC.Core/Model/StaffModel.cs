using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITC.Data.Enum;

namespace ITC.Core.Model
{
    public class StaffModel
    {
        public Guid? Id { get; set; }
        public string? StaffCode { get; set; }
        public bool IsHeadOfDepartMent { get; set; }
    }

    public class CreateStaffModel
    {
        public string? StaffCode { get; set; }
        public Guid? AccountId { get; set; }
    }

   
    public class StaffViewModel
    {
        public Guid Id { get; set; }
        public string? StaffCode { get; set; }
        public bool IsHeadOfDepartMent { get; set; }
        public Guid? AccountId { get; set; }
        public AccountModel? Account { get; set; }
    }

    public class UpdateStaffModel
    {
        public string? StaffCode { get; set; }
        public bool IsHeadOfDepartMent { get; set; }
        public Guid? AccountId { get; set; }
    }

}
