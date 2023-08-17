using System;
using System.ComponentModel.DataAnnotations;
using ITC.Data.Enum;

namespace ITC.Core.Model
{
	public class LoginRequest
	{
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

     public class LoginReponse
     {
        public Guid? Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public RoleEnum Role { get; set; }
        public string? UrlAvatar { get; set; }
        public string? AccountToken { get; set; }
        public bool Status { get; set; }
        public StaffModel? Staff { get; set; }
    }

}

