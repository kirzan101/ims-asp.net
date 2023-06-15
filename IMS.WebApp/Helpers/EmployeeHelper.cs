using Dapper;
using IMS.Shared.Data.Interface;
using IMS.Shared.Helpers;
using IMS.Shared.Models;
using IMS.Shared.ViewModels.Pagination;
using IMS.Shared.ViewModels.Requests;

namespace IMS.WebApp.Helpers
{
    public class EmployeeHelper
    {
        private readonly IEmployeeService _service;
        private readonly DBCommInfo dBCommInfo;

        public EmployeeHelper(IEmployeeService service)
        {
            _service = service;
            dBCommInfo = new DBCommInfo();
            dBCommInfo.TableName = "Employees";
            dBCommInfo.SP_Paged_Name = "SP_PAGED_Employees";
            dBCommInfo.SP_Crud_Name = "SP_CRUD_Employees";
        }

        public async Task<PagedResultDTO<EmployeePagedDTO>> GetEmployeeList(PagedQueryDTO dto)
        {
            PagedResultDTO<EmployeePagedDTO>? employees = new PagedResultDTO<EmployeePagedDTO>();
            dto.Status = 1;

            var result = await _service.GetPaged(dBCommInfo.SP_Paged_Name, dto);
            if (result.responseCode == 204) return employees;

            if (result.responseData != null)
            {
                employees = result.responseData;
            }

            return employees;
        }

        public async Task<Employee> GetEmployeeById(int id)
        {
            Employee employee = new Employee();

            var result = await _service.GetByIdAsync(dBCommInfo.TableName, id);
            if (result.responseCode == 204) return employee;

            if(result.responseData != null)
            {
                employee = result.responseData;
            }

            return employee;

        }

        public async Task<bool> Create(EmployeeDTO model)
        {
            var parameters = new DynamicParameters();
            parameters.AddDynamicParams(model);
            var result = await _service.CreateAsync(dBCommInfo, parameters);

            return result.isSuccess;
        }

        public async Task<bool> Update(int id, EmployeeDTO model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@id", id);
            parameters.AddDynamicParams(model);

            var result = await _service.UpdateAsync(dBCommInfo, parameters, id);

            return result.isSuccess;
        }
    }
}
