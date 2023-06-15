using Dapper;
using IMS.Shared.Helpers;
using IMS.Shared.ViewModels.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Shared.Data.Interface
{
    public interface IBasePagedCrudService<TModel, TPagedModel> : IBaseCrudService<TModel>
        where TModel : class
        where TPagedModel : class
    {
        Task<GenericResult<PagedResultDTO<TPagedModel>>> GetPaged(string SPName, PagedQueryDTO query);
        Task<GenericResult<PagedResultDTO<TPagedModel>>> GetDynamicPaged(string SPName, DynamicParameters dynamicParam);
    }
}
