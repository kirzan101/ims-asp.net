using Dapper;
using IMS.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Shared.Data.Interface
{
    public interface IBaseCrudService<TModel> where TModel : class
    {
        Task<GenericResult<TModel>> CreateAsync(DBCommInfo dbInfo, DynamicParameters Params);
        Task<GenericResult<TModel>> CreateAsync(IDbConnection db, IDbTransaction transaction, DBCommInfo dbInfo, DynamicParameters Params);
        Task<GenericResult<string>> CreateMutipleAsync<TRequestDto>(DBCommInfo dbInfo, List<TRequestDto> modelRecords);
        Task<GenericResult<List<TModel>>> GetAllRecordInfo(string tableName, string whereClause = "", string? spName = null, string? fieldnames = null, string? orderByClause = null);
        Task<GenericResult<List<TModel>>> GetAllRecordInfo(IDbConnection db, IDbTransaction transaction, string tableName, string whereClause = "", string? spName = null, string? fieldnames = null, string? orderByClause = null);

        Task<GenericResult<TModel>> GetByFieldAsync(string tableName, string fieldName, string value, string order = " ASC ", int? limit = null, bool checkStatus = false);
        Task<GenericResult<TModel>> GetByFieldIntAsync(string tableName, string field, int value, string order = " ASC ", int? limit = null, int statusFlag = 1);
        Task<GenericResult<TModel>> GetByFieldIntAsync(IDbConnection db, IDbTransaction transaction, string tableName, string field, int value, string order = " ASC ", int? limit = null);
        Task<GenericResult<TModel>> GetByIdAsync(string tableName, int id, bool checkStatus = true);
        Task<GenericResult<TModel>> GetByIdAsync(IDbConnection db, IDbTransaction transaction, string tableName, int id, bool checkStatus = true);

        Task<GenericResult<List<TModel>>> GetListByFieldIntAsync(string tableName, string field, int value, string order = " ASC ", int? limit = null, bool checkStatus = false);
        Task<GenericResult<List<int>>> GetDistinctInt(string tableName, string distinctIntField, string? whereField = null, int? whereValue = null, string? additionalFieldInt = null);
        Task<GenericResult<List<string>>> GetDistinctStr(string tableName, string distinctStrField, string? whereField = null, int? whereValue = null);
        /// <summary>
        /// Generic method to update status fields: Active and Delete
        /// </summary>
        /// <param name="tableName">Table to apply the update</param>
        /// <param name="optionFlag">Flag to specify what kind of status update to be applied: 0 = Active; 1 = Inactive; 2 = Soft Deleted; 3 = Purge</param>
        /// <param name="id">Id of the record to be updated</param>
        /// <param name="modifierId">Id of the user who issued the update. Optional for PURGE(optionFlag = 3).</param>
        /// <param name="IdFieldName">Field name of the Id to be used as filter. Optional defualts to empty string.</param>
        /// <returns>Success or Error Message</returns>
        Task<GenericResult<string>> UpdateStatusAsync(string tableName, int optionFlag, int id, int? modifierId, string IdFieldName = "");
        Task<GenericResult<string>> UpdateStatusAsync(IDbConnection db, IDbTransaction transaction, string tableName, int optionFlag, int id, int? modifierId, string IdFieldName = "");

        Task<GenericResult<TModel>> UpdateAsync(DBCommInfo dbInfo, DynamicParameters Params, int id);
        Task<GenericResult<TModel>> UpdateAsync(IDbConnection db, IDbTransaction transaction, DBCommInfo dbInfo, DynamicParameters Params, int id);
    }
}
