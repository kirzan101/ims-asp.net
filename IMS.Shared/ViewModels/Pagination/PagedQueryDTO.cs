using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Shared.ViewModels.Pagination
{
    public class PagedQueryDTO
    {
        [DefaultValue(1)]
        public int? Index { get; set; } = 1;
        [DefaultValue(25)]
        public int? PageSize { get; set; } = 25;
        public string? Filter { get; set; }
        [DefaultValue("Id")]
        public string OrderField { get; set; } = "Id";
        [DefaultValue(" ASC ")]
        public string OrderDirection { get; set; } = " ASC ";
        [DefaultValue(null)]
        public int? Status { get; set; } = null;
    }
}
