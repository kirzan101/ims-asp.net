using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Shared.Models
{
    public class Employee : BaseWithStatus
    {
        public string Salutation { get; set; } = string.Empty;
        public string NickName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Suffix { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
    }
}
