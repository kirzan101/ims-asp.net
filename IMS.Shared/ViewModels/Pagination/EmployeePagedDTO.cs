using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Shared.ViewModels.Pagination
{
    public class EmployeePagedDTO : BasePagedDTOWithStatus
    {
        public string? Salutation { get; set; }
        public string? Name { get; set; }
        public string? NickName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Suffix { get; set; }
        public string? MobileNumber { get; set; }
        public string? Designation { get; set; }
        public string? Branch { get; set; }
        public string? ImagePath { get; set; }
    }
}
