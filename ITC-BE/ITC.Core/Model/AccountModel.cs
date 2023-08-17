using System;
using System.ComponentModel.DataAnnotations;
using ITC.Data.Entities;
using ITC.Data.Enum;

namespace ITC.Core.Model
{
    public class AccountModel
    {
        public Guid? AccountId { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public string? FullName { get; set; }
        public string? UrlAvatar { get; set; }
        public DateTime? BirthDay { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public bool Status { get; set; }
        public RoleEnum Role { get; set; }
        public DateTime? DateCreated { get; set; }
    }

    public class AccountViewModel
    {
        public Guid? AccountId { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public string? FullName { get; set; }
        public string? UrlAvatar { get; set; }
        public string? FireBaseToken { get; set; }
        public DateTime? BirthDay { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public bool Status { get; set; }
        public RoleEnum Role { get; set; }
        public DateTime? DateCreated { get; set; }
    }

    public class ViewCurrentAccountModel
    {
        public Guid? AccountId { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public RoleEnum Role { get; set; }
    }



    public class CreateNewAccountModel
    {
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid Email address.")]
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? UrlAvatar { get; set; }
        public DateTime? BirthDay { get; set; }
        [DataType(DataType.PhoneNumber, ErrorMessage = "Provided phone number not valid")]
        [StringLength(10, MinimumLength = 10)]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Phone Number.")]
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }

        public RoleEnum Role { get; set; }
    }

    public class UpdateAccountModel
    {
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid Email address.")]
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public string? FullName { get; set; }
        public string? UrlAvatar { get; set; }
        public DateTime? BirthDay { get; set; }
        [DataType(DataType.PhoneNumber, ErrorMessage = "Provided phone number not valid")]
        [StringLength(10, MinimumLength = 10)]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Phone Number.")]
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public bool Status { get; set; }

        public RoleEnum Role { get; set; }
    }

    public class ChangePasswordModel
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? OldPassword { get; set; }
        [Required]
        public string? NewPassword { get; set; }
        [Required]
        public string? ConfirmPassword { get; set; }
    }
}



