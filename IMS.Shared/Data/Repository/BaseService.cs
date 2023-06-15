using IMS.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Shared.Data.Repository
{
    public class BaseService
    {
        private readonly AppSettings _appSettings;
        public BaseService(AppSettings appSettings)
        {
            _appSettings = appSettings;

        }
        public GenericResult<SqlConnection> GetConnection()
        {
            SqlConnection connection;
            try
            {
                connection = new SqlConnection(_appSettings.ConnectionString);
                connection.Open();
            }
            catch (Exception ex)
            {
                return new GenericResult<SqlConnection>(false, ex.Message, response: (int)HttpStatusCode.InternalServerError);
            }
            return new GenericResult<SqlConnection>(true, "Success", connection);
        }
    }
}
