using Dapper;
using IMS.Shared.Data.Interface;
using IMS.Shared.Helpers;
using IMS.Shared.ViewModels.Pagination;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Shared.Data.Repository
{
    public class BasePagedCrudService<TModel, TPagedModel> : BaseCrudService<TModel>, IBasePagedCrudService<TModel, TPagedModel>
        where TModel : class
        where TPagedModel : class
    {
        public BasePagedCrudService(AppSettings appSettings) : base(appSettings)
        {
        }

        public async Task<GenericResult<PagedResultDTO<TPagedModel>>> GetDynamicPaged(string SPName, DynamicParameters dynamicParam)
        {
            var connInfo = GetConnection();
            if (!connInfo.isSuccess)
                return new GenericResult<PagedResultDTO<TPagedModel>>(connInfo.isSuccess, connInfo.message, response: connInfo.responseCode);

            using (var db = connInfo.responseData as IDbConnection)
            {
                try
                {
                    //run all queries in parallel asynchronously
                    var pagedTask = db.QueryAsync<TPagedModel>(SPName, dynamicParam, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    var filteredRCTask = GetFilteredRowCountAsync(SPName, dynamicParam);
                    var totalRowsTask = GetTotalRowCountAsync(SPName);

                    var pagedResult = await pagedTask;
                    var filteredRCResult = await filteredRCTask;
                    var totalRowsResult = await totalRowsTask;

                    PagedResultDTO<TPagedModel> result = new PagedResultDTO<TPagedModel>();
                    result.pagedList = pagedResult;
                    result.recordsFiltered = filteredRCResult;
                    //result.recordsFiltered = pagedResult.AsList().Count;
                    result.totalrows = totalRowsResult;

                    if (result.recordsFiltered < 1)
                        return new GenericResult<PagedResultDTO<TPagedModel>>(true, "No Records Found.", response: (int)HttpStatusCode.NoContent);

                    return new GenericResult<PagedResultDTO<TPagedModel>>(true, "success", result);
                }
                catch (Exception ex)
                {
                    return new GenericResult<PagedResultDTO<TPagedModel>>(false, ex.Message, response: (int)HttpStatusCode.InternalServerError);
                }
            }
        }

        public async Task<GenericResult<PagedResultDTO<TPagedModel>>> GetPaged(string SPName, PagedQueryDTO query)
        {
            var parameters = new DynamicParameters();

            var connInfo = GetConnection();
            if (!connInfo.isSuccess)
                return new GenericResult<PagedResultDTO<TPagedModel>>(connInfo.isSuccess, connInfo.message, response: connInfo.responseCode);

            using (var db = connInfo.responseData as IDbConnection)
            {
                try
                {
                    parameters.Add("@Index", query.Index);
                    parameters.Add("@PageSize", query.PageSize);
                    parameters.Add("@Filter", query.Filter);
                    parameters.Add("@OrderField", query.OrderField);
                    parameters.Add("@OrderDirection", query.OrderDirection);
                    if (query.Status != null) parameters.Add("@Status", query.Status);

                    //run all queries in parallel asynchronously
                    var pagedTask = db.QueryAsync<TPagedModel>(SPName, parameters, commandType: CommandType.StoredProcedure);
                    var filteredRCTask = GetFilteredRowCountAsync(SPName, parameters);
                    var totalRowsTask = GetTotalRowCountAsync(SPName);

                    var pagedResult = await pagedTask;
                    var filteredRCResult = await filteredRCTask;
                    var totalRowsResult = await totalRowsTask;

                    PagedResultDTO<TPagedModel> result = new PagedResultDTO<TPagedModel>();
                    result.pagedList = pagedResult;
                    result.recordsFiltered = filteredRCResult;
                    //result.recordsFiltered = pagedResult.AsList().Count;
                    result.totalrows = totalRowsResult;

                    if (result.recordsFiltered < 1)
                        return new GenericResult<PagedResultDTO<TPagedModel>>(true, "No Records Found.", response: (int)HttpStatusCode.NoContent);

                    return new GenericResult<PagedResultDTO<TPagedModel>>(true, "success", result);
                }
                catch (Exception ex)
                {
                    return new GenericResult<PagedResultDTO<TPagedModel>>(false, ex.Message, response: (int)HttpStatusCode.InternalServerError);
                }
            }

        }

        protected async Task<int> GetFilteredRowCountAsync(string spName, DynamicParameters dynamicParam)
        {
            var connInfo = GetConnection();
            if (!connInfo.isSuccess) return -1;



            using (var db = connInfo.responseData as IDbConnection)
            {
                try
                {
                    //for filter row result
                    dynamicParam.Add("@init_flag", 1);
                    var result = await db.QueryFirstOrDefaultAsync<int>(spName, dynamicParam, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    return result;
                }
                catch (Exception ex)
                {
                    Debug.Write(ex.Message);
                    return -1;
                }
            }
        }

        protected async Task<int> GetTotalRowCountAsync(string spName)
        {
            var connInfo = GetConnection();
            if (!connInfo.isSuccess) return -1;

            using (var db = connInfo.responseData as IDbConnection)
            {
                try
                {
                    var parameters = new DynamicParameters(new { init_flag = 2 });
                    var result = await db.QueryFirstOrDefaultAsync<int>(spName, parameters, commandType: CommandType.StoredProcedure);
                    return result;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return -1;
                }
            }
        }
    }
}
