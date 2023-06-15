using Dapper;
using IMS.Shared.Data.Interface;
using IMS.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Shared.Data.Repository
{
    public class BaseCrudService<TModel> : BaseService, IBaseCrudService<TModel> where TModel : class
    {
        public BaseCrudService(AppSettings appSettings) : base(appSettings)
        {
        }

        public async Task<GenericResult<TModel>> CreateAsync(DBCommInfo dbInfo, DynamicParameters Params)
        {
            var connInfo = GetConnection();
            if (!connInfo.isSuccess)
                return new GenericResult<TModel>(connInfo.isSuccess, connInfo.message, response: connInfo.responseCode);

            using (var db = connInfo.responseData as IDbConnection)
            {
                try
                {
                    Params.Add("@ret_errmsg", dbType: DbType.String, direction: ParameterDirection.Output, size: 80);
                    Params.Add("@init_flag", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                    Params.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                    var result = await db.ExecuteAsync(dbInfo.SP_Crud_Name, Params, commandType: CommandType.StoredProcedure);
                    var ret_val = Params.Get<int>("RetVal");
                    var out_message = Params.Get<string>("ret_errmsg");
                    int last_id = Params.Get<int>("init_flag");

                    if (ret_val != 0 || last_id < 0)
                        return new GenericResult<TModel>(false, "", response: (int)HttpStatusCode.InternalServerError);


                    var newRecord = await GetByIdAsync(dbInfo.TableName, last_id, false);

                    return newRecord;
                }
                catch (Exception ex)
                {
                    return new GenericResult<TModel>(false, ex.Message, response: (int)HttpStatusCode.InternalServerError);
                }
            }
        }

        public async Task<GenericResult<TModel>> CreateAsync(IDbConnection db, IDbTransaction transaction, DBCommInfo dbInfo, DynamicParameters Params)
        {
            try
            {
                Params.Add("@ret_errmsg", dbType: DbType.String, direction: ParameterDirection.Output, size: 80);
                Params.Add("@init_flag", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                Params.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                var result = await db.ExecuteAsync(dbInfo.SP_Crud_Name, Params, transaction, commandType: CommandType.StoredProcedure);
                var ret_val = Params.Get<int>("RetVal");
                var out_message = Params.Get<string>("ret_errmsg");
                int last_id = Params.Get<int>("init_flag");

                if (ret_val != 0 || last_id < 0)
                    return new GenericResult<TModel>(false, "", response: (int)HttpStatusCode.InternalServerError);


                var newRecord = await GetByIdAsync(db, transaction, dbInfo.TableName, last_id, false);

                return newRecord;
            }
            catch (Exception ex)
            {
                return new GenericResult<TModel>(false, ex.Message, response: (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<GenericResult<string>> CreateMutipleAsync<TRequestDto>(DBCommInfo dbInfo, List<TRequestDto> modelRecords)
        {
            if (modelRecords.Count <= 0) new GenericResult<string>(false, "Invalid Input", response: (int)HttpStatusCode.BadRequest);

            var connInfo = GetConnection();
            if (!connInfo.isSuccess)
                return new GenericResult<string>(connInfo.isSuccess, connInfo.message, response: connInfo.responseCode);

            using (var db = connInfo.responseData as IDbConnection)
            {
                string out_message = "Something went wrong.";
                int last_id = -1;

                try
                {
                    using (var transaction = db?.BeginTransaction())
                    {
                        foreach (var record in modelRecords)
                        {
                            DynamicParameters dparams = new DynamicParameters();
                            dparams.AddDynamicParams(record);
                            dparams.Add("@ret_errmsg", dbType: DbType.String, direction: ParameterDirection.Output, size: 80);
                            dparams.Add("@init_flag", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                            dparams.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                            await db.ExecuteAsync(dbInfo.SP_Crud_Name, dparams, transaction, commandType: CommandType.StoredProcedure);
                            last_id = dparams.Get<int>("init_flag");
                            out_message = dparams.Get<string>("ret_errmsg");

                            if (last_id <= 0)
                            {
                                transaction?.Rollback();
                                return new GenericResult<string>(false, out_message, response: (int)HttpStatusCode.InternalServerError);
                            }
                        }

                        out_message += $" {modelRecords.Count} Record(s) Created.";
                        transaction?.Commit();
                        return new GenericResult<string>(false, out_message, response: (int)HttpStatusCode.InternalServerError); ;
                    }
                }
                catch (Exception ex)
                {
                    return new GenericResult<string>(false, ex.Message, response: (int)HttpStatusCode.InternalServerError);
                }
            }
        }

        public async Task<GenericResult<List<TModel>>> GetAllRecordInfo(string tableName, string whereClause = "", string? spName = null, string? fieldnames = null, string? orderByClause = null)
        {
            var connInfo = GetConnection();
            if (!connInfo.isSuccess)
                return new GenericResult<List<TModel>>(connInfo.isSuccess, connInfo.message, response: connInfo.responseCode);

            spName = spName ?? "SP_CRUD_GetAllRecords";

            using (var db = connInfo.responseData as IDbConnection)
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@ret_errmsg", dbType: DbType.String, direction: ParameterDirection.Output, size: 80);
                    parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                    parameters.Add("@tableName", tableName);
                    if (!string.IsNullOrEmpty(fieldnames)) parameters.Add("@fieldnames", fieldnames);
                    if (!string.IsNullOrEmpty(orderByClause)) parameters.Add("@orderByClause", orderByClause);
                    if (!string.IsNullOrEmpty(whereClause))
                        parameters.Add("@WhereClause", whereClause);


                    var result = await db.QueryAsync<TModel>(spName, parameters, commandType: CommandType.StoredProcedure);

                    if (result == null || result.Count() <= 0)
                        return new GenericResult<List<TModel>>(true, "", response: (int)HttpStatusCode.NoContent);

                    return new GenericResult<List<TModel>>(true, "Successful", result.ToList());
                }
                catch (Exception ex)
                {
                    return new GenericResult<List<TModel>>(false, ex.Message, response: (int)HttpStatusCode.InternalServerError);
                }
            }
        }

        // With transaction
        public async Task<GenericResult<List<TModel>>> GetAllRecordInfo(IDbConnection db, IDbTransaction transaction, string tableName, string whereClause = "", string? spName = null, string? fieldnames = null, string? orderByClause = null)
        {
            spName = spName ?? "SP_CRUD_GetAllRecords";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ret_errmsg", dbType: DbType.String, direction: ParameterDirection.Output, size: 80);
                parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                parameters.Add("@tableName", tableName);
                if (!string.IsNullOrEmpty(fieldnames)) parameters.Add("@fieldnames", fieldnames);
                if (!string.IsNullOrEmpty(orderByClause)) parameters.Add("@orderByClause", orderByClause);
                if (!string.IsNullOrEmpty(whereClause))
                    parameters.Add("@WhereClause", whereClause);


                var result = await db.QueryAsync<TModel>(spName, parameters, transaction, commandType: CommandType.StoredProcedure);

                if (result == null || result.Count() <= 0)
                    return new GenericResult<List<TModel>>(true, "", response: (int)HttpStatusCode.NoContent);

                return new GenericResult<List<TModel>>(true, "Successful", result.ToList());
            }
            catch (Exception ex)
            {
                return new GenericResult<List<TModel>>(false, ex.Message, response: (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<GenericResult<TModel>> GetByFieldAsync(string tableName, string fieldName, string value, string order = " ASC ", int? limit = null, bool checkStatus = false)
        {
            var connInfo = GetConnection();
            if (!connInfo.isSuccess)
                return new GenericResult<TModel>(connInfo.isSuccess, connInfo.message, response: connInfo.responseCode);

            using (var db = connInfo.responseData as IDbConnection)
            {
                try
                {
                    var result = await db.QueryFirstOrDefaultAsync<TModel>(
                        "[dbo].[SP_CRUD_GetByFieldString]",
                        new { tableName, field = fieldName, value, order, limit, checkStatus },
                        commandType: CommandType.StoredProcedure);

                    if (result == null) return new GenericResult<TModel>(true, "No Records Found", response: (int)HttpStatusCode.NoContent);
                    return new GenericResult<TModel>(true, "success", result);
                }
                catch (Exception ex)
                {
                    return new GenericResult<TModel>(false, ex.Message, response: (int)HttpStatusCode.InternalServerError);
                }
            }
        }

        public async Task<GenericResult<TModel>> GetByFieldIntAsync(string tableName, string field, int value, string order = " ASC ", int? limit = null, int statusFlag = 1)
        {
            var connInfo = GetConnection();
            if (!connInfo.isSuccess)
                return new GenericResult<TModel>(connInfo.isSuccess, connInfo.message, response: connInfo.responseCode);

            using (var db = connInfo.responseData as IDbConnection)
            {
                try
                {
                    var result = await db.QueryFirstOrDefaultAsync<TModel>(
                        "[dbo].[SP_CRUD_GetByFieldInt]",
                        new { tableName, field, value, order, limit, statusFlag },
                        commandType: CommandType.StoredProcedure);

                    if (result == null) return new GenericResult<TModel>(true, "No Records Found", response: (int)HttpStatusCode.NoContent);
                    return new GenericResult<TModel>(true, "success", result);
                }
                catch (Exception ex)
                {
                    return new GenericResult<TModel>(false, ex.Message, response: (int)HttpStatusCode.InternalServerError);
                }
            }
        }

        public async Task<GenericResult<TModel>> GetByFieldIntAsync(IDbConnection db, IDbTransaction transaction, string tableName, string field, int value, string order = " ASC ", int? limit = null)
        {
            try
            {
                var result = await db.QueryFirstOrDefaultAsync<TModel>(
                    "[dbo].[SP_CRUD_GetByFieldInt]",
                    new { tableName, field, value, order, limit }, transaction,
                    commandType: CommandType.StoredProcedure);

                if (result == null) return new GenericResult<TModel>(true, "No Records Found", response: (int)HttpStatusCode.NoContent);
                return new GenericResult<TModel>(true, "success", result);
            }
            catch (Exception ex)
            {
                return new GenericResult<TModel>(false, ex.Message, response: (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<GenericResult<TModel>> GetByIdAsync(string tableName, int id, bool checkStatus = true)
        {
            var connInfo = GetConnection();
            if (!connInfo.isSuccess)
                return new GenericResult<TModel>(connInfo.isSuccess, connInfo.message, response: connInfo.responseCode);

            using (var db = connInfo.responseData as IDbConnection)
            {
                try
                {
                    var result = await db.QueryFirstOrDefaultAsync<TModel>(
                        "[dbo].[SP_CRUD_GetByFieldInt]",
                        new { tableName, field = "Id", value = id, checkStatus },
                        commandType: CommandType.StoredProcedure);

                    if (result == null) return new GenericResult<TModel>(true, "No Records Found", response: (int)HttpStatusCode.NoContent);
                    return new GenericResult<TModel>(true, "success", result);
                }
                catch (Exception ex)
                {
                    return new GenericResult<TModel>(false, ex.Message, response: (int)HttpStatusCode.InternalServerError);
                }
            }
        }

        public async Task<GenericResult<TModel>> GetByIdAsync(IDbConnection db, IDbTransaction transaction, string tableName, int id, bool checkStatus = true)
        {
            try
            {
                var result = await db.QueryFirstOrDefaultAsync<TModel>(
                    "[dbo].[SP_CRUD_GetByFieldInt]",
                    new { tableName, field = "Id", value = id, checkStatus },
                    transaction,
                    commandType: CommandType.StoredProcedure);

                if (result == null) return new GenericResult<TModel>(true, "No Records Found", response: (int)HttpStatusCode.NoContent);
                return new GenericResult<TModel>(true, "success", result);
            }
            catch (Exception ex)
            {
                return new GenericResult<TModel>(false, ex.Message, response: (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<GenericResult<List<TModel>>> GetListByFieldIntAsync(string tableName, string field, int value, string order = " ASC ", int? limit = null, bool checkStatus = false)
        {
            var connInfo = GetConnection();
            if (!connInfo.isSuccess)
                return new GenericResult<List<TModel>>(connInfo.isSuccess, connInfo.message, response: connInfo.responseCode);

            using (var db = connInfo.responseData as IDbConnection)
            {
                try
                {
                    var result = await db.QueryAsync<TModel>(
                        "[dbo].[SP_CRUD_GetByFieldInt]",
                        new { tableName, field, value, order, limit, checkStatus },
                        commandType: CommandType.StoredProcedure);

                    if (result == null || result.Count() <= 0)
                        return new GenericResult<List<TModel>>(true, "", response: (int)HttpStatusCode.NoContent);

                    return new GenericResult<List<TModel>>(true, "success", result.ToList());
                }
                catch (Exception ex)
                {
                    return new GenericResult<List<TModel>>(false, ex.Message, response: (int)HttpStatusCode.InternalServerError);
                }
            }
        }


        /// <summary>
        /// Generic method to update status fields: Active and Delete
        /// </summary>
        /// <param name="tableName">Table to apply the update</param>
        /// <param name="optionFlag">Flag to specify what kind of status update to be applied: 0 = Active; 1 = Inactive; 2 = Soft Deleted; 3 = Purge</param>
        /// <param name="id">Id of the record to be updated</param>
        /// <param name="modifierId">Id of the user who issued the update. Optional for PURGE(optionFlag = 3).</param>
        /// <param name="IdFieldName">Field name of the Id to be used as filter. Optional defualts to empty string.</param>
        /// <returns>Success or Error Message</returns>
        public async Task<GenericResult<string>> UpdateStatusAsync(string tableName, int optionFlag, int id, int? modifierId, string IdFieldName = "")
        {
            var connInfo = GetConnection();
            if (!connInfo.isSuccess)
                return new GenericResult<string>(connInfo.isSuccess, connInfo.message, response: connInfo.responseCode);

            using (var db = connInfo.responseData as IDbConnection)
            {
                try
                {
                    var Params = new DynamicParameters();

                    Params.Add("@ret_errmsg", dbType: DbType.String, direction: ParameterDirection.Output, size: 80);
                    Params.Add("@init_flag", optionFlag, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                    Params.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                    Params.Add("@tableName", tableName);
                    Params.Add("@Id", id);
                    Params.Add("@UserId", modifierId);
                    if (!string.IsNullOrEmpty(IdFieldName)) Params.Add("@IdFieldName", IdFieldName);

                    var result = await db.ExecuteAsync("SP_CRUD_StatusUpdate", Params, commandType: CommandType.StoredProcedure);
                    var ret_val = Params.Get<int>("RetVal");
                    var out_message = Params.Get<string>("ret_errmsg");

                    if (ret_val != 0) return new GenericResult<string>(false, connInfo.message, response: (int)HttpStatusCode.InternalServerError);

                    return new GenericResult<string>(true, connInfo.message);

                }
                catch (Exception ex)
                {
                    return new GenericResult<string>(false, ex.Message, response: (int)HttpStatusCode.InternalServerError);
                }
            }
        }

        public async Task<GenericResult<string>> UpdateStatusAsync(IDbConnection db, IDbTransaction transaction, string tableName, int optionFlag, int id, int? modifierId, string IdFieldName = "")
        {
            try
            {
                var Params = new DynamicParameters();

                Params.Add("@ret_errmsg", dbType: DbType.String, direction: ParameterDirection.Output, size: 80);
                Params.Add("@init_flag", optionFlag, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                Params.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                Params.Add("@tableName", tableName);
                Params.Add("@Id", id);
                Params.Add("@UserId", modifierId);
                if (!string.IsNullOrEmpty(IdFieldName)) Params.Add("@IdFieldName", IdFieldName);

                var result = await db.ExecuteAsync("SP_CRUD_StatusUpdate", Params, transaction, commandType: CommandType.StoredProcedure);
                var ret_val = Params.Get<int>("RetVal");
                var out_message = Params.Get<string>("ret_errmsg");

                if (ret_val != 0) return new GenericResult<string>(false, out_message, response: (int)HttpStatusCode.InternalServerError);

                return new GenericResult<string>(true, out_message);

            }
            catch (Exception ex)
            {
                return new GenericResult<string>(false, ex.Message, response: (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<GenericResult<TModel>> UpdateAsync(DBCommInfo dbInfo, DynamicParameters Params, int id)
        {
            var connInfo = GetConnection();
            if (!connInfo.isSuccess)
                return new GenericResult<TModel>(connInfo.isSuccess, connInfo.message, response: connInfo.responseCode);

            using (var db = connInfo.responseData as IDbConnection)
            {
                try
                {
                    Params.Add("@ret_errmsg", dbType: DbType.String, direction: ParameterDirection.Output, size: 80);
                    Params.Add("@init_flag", 1, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                    Params.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                    var result = await db.ExecuteAsync(dbInfo.SP_Crud_Name, Params, commandType: CommandType.StoredProcedure);
                    var ret_val = Params.Get<int>("RetVal");
                    var out_message = Params.Get<string>("ret_errmsg");

                    if (ret_val != 0) return new GenericResult<TModel>(false, "", response: (int)HttpStatusCode.InternalServerError);

                    var updatedRecord = await GetByIdAsync(dbInfo.TableName, id, false);

                    return updatedRecord;
                }
                catch (Exception ex)
                {
                    return new GenericResult<TModel>(false, ex.Message, response: (int)HttpStatusCode.InternalServerError);
                }
            }
        }

        public async Task<GenericResult<TModel>> UpdateAsync(IDbConnection db, IDbTransaction transaction, DBCommInfo dbInfo, DynamicParameters Params, int id)
        {
            try
            {
                Params.Add("@ret_errmsg", dbType: DbType.String, direction: ParameterDirection.Output, size: 80);
                Params.Add("@init_flag", 1, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                Params.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                var result = await db.ExecuteAsync(dbInfo.SP_Crud_Name, Params, transaction, commandType: CommandType.StoredProcedure);
                var ret_val = Params.Get<int>("RetVal");
                var out_message = Params.Get<string>("ret_errmsg");

                if (ret_val != 0) return new GenericResult<TModel>(false, "", response: (int)HttpStatusCode.InternalServerError);

                var updatedRecord = await GetByIdAsync(db, transaction, dbInfo.TableName, id, false);

                return updatedRecord;
            }
            catch (Exception ex)
            {
                return new GenericResult<TModel>(false, ex.Message, response: (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<GenericResult<List<int>>> GetDistinctInt(string tableName, string distinctIntField, string? whereField = null, int? whereValue = null, string? additionalFields = null)
        {
            var connInfo = GetConnection();
            if (!connInfo.isSuccess)
                return new GenericResult<List<int>>(connInfo.isSuccess, connInfo.message, response: connInfo.responseCode);

            using (var db = connInfo.responseData as IDbConnection)
            {
                try
                {
                    var result = await db.QueryAsync<int>(
                        "[dbo].[SP_CRUD_GetDistinctInt]",
                        new { tableName, distinctIntField, whereField, whereValue, additionalFields },
                        commandType: CommandType.StoredProcedure);

                    if (result == null || result?.Count() == 0) return new GenericResult<List<int>>(true, "No Records Found", response: (int)HttpStatusCode.NoContent);
                    return new GenericResult<List<int>>(true, "success", result?.ToList());
                }
                catch (Exception ex)
                {
                    return new GenericResult<List<int>>(false, ex.Message, response: (int)HttpStatusCode.InternalServerError);
                }
            }
        }

        public async Task<GenericResult<List<string>>> GetDistinctStr(string tableName, string distinctStrField, string? whereField = null, int? whereValue = null)
        {
            var connInfo = GetConnection();
            if (!connInfo.isSuccess)
                return new GenericResult<List<string>>(connInfo.isSuccess, connInfo.message, response: connInfo.responseCode);

            using (var db = connInfo.responseData as IDbConnection)
            {
                try
                {
                    var result = await db.QueryAsync<string>(
                        "[dbo].[SP_CRUD_GetDistinctString]",
                        new { tableName, distinctStrField, whereField, whereValue },
                        commandType: CommandType.StoredProcedure);

                    if (result == null || result?.Count() == 0) return new GenericResult<List<string>>(true, "No Records Found", response: (int)HttpStatusCode.NoContent);
                    return new GenericResult<List<string>>(true, "success", result?.ToList());
                }
                catch (Exception ex)
                {
                    return new GenericResult<List<string>>(false, ex.Message, response: (int)HttpStatusCode.InternalServerError);
                }
            }
        }
    }
}
