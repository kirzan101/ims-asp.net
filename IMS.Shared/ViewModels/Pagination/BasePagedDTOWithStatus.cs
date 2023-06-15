using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Shared.ViewModels.Pagination
{
    public class BasePagedDTOWithStatus : BasePagedDTO
    {
        public bool Active { get; set; } = true;
        public bool Deleted { get; set; } = false;
    }
}
