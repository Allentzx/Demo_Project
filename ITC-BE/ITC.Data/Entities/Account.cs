using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ITC.Data.Enum;

namespace ITC.Data.Entities
{
    public class Account
    {
        public Guid? AccountId { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public string? UrlAvatar { get; set; }
        public string? FireBaseToken { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? BirthDay { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public bool Status { get; set; }

        public RoleEnum Role { get; set; }

        public Deputy? Deputy { get; set; }
        public Staff? Staff { get; set; }

    }
}

