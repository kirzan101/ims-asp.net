using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Shared.ViewModels.Pagination
{
    public class PagedResultDTO<Model>
    {
        public IEnumerable<Model>? pagedList { get; set; }
        public int totalrows { get; set; }
        public int recordsFiltered { get; set; }
        public object? otherInfo { get; set; }
    }
}
