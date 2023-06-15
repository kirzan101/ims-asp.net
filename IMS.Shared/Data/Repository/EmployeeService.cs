using IMS.Shared.Data.Interface;
using IMS.Shared.Helpers;
using IMS.Shared.Models;
using IMS.Shared.ViewModels.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Shared.Data.Repository
{
    public class EmployeeService : BasePagedCrudService<Employee, EmployeePagedDTO>, IEmployeeService
    {
        public EmployeeService(AppSettings appSettings) : base(appSettings)
        {
        }
    }
}
