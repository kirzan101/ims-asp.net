using IMS.Shared.Helpers;
using IMS.Shared.Models;
using IMS.Shared.ViewModels.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Shared.Data.Interface
{
    public interface IEmployeeService : IBasePagedCrudService<Employee, EmployeePagedDTO>
    {
    }
}
