using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Shared.ViewModels.Requests
{
    public class EmployeeDTO
    {
        public string? Salutation { get; set; }
        public string? NickName { get; set; }
        [Required]
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        [Required]
        public string? LastName { get; set; }
        public string? Suffix { get; set; }
        public string? MobileNumber { get; set; }
        [Required]
        public string? Designation { get; set; }
        [Required]
        public string? Branch { get; set; }
        public string? ImagePath { get; set; }
        [Required]
        public int UserId { get; set; } // creator or modifier of the record
        public bool Active { get; set; } = true;
    }
}
