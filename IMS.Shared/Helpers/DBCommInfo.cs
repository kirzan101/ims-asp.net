using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Shared.Helpers
{
    public class DBCommInfo
    {
        public string TableName { get; set; } = string.Empty;
        public string SP_Crud_Name { get; set; } = string.Empty;
        public string SP_Paged_Name { get; set; } = string.Empty;
    }
}
