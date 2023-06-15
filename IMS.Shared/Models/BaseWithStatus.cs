using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Shared.Models
{
    public class BaseWithStatus : Base
    {
        public bool Active { get; set; } = true;
        public bool Deleted { get; set; } = false;
    }
}
