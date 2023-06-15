using IMS.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Shared.Data.Interface
{
    public interface IBaseService
    {
        GenericResult<SqlConnection> GetConnection();
    }
}
