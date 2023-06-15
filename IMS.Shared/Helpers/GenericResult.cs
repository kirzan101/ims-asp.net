using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Shared.Helpers
{
    public class GenericResult<TData> where TData : class
    { 
        public GenericResult(bool isSuccess, string msg, TData? data = null, int response = 200)
        {
            this.isSuccess = isSuccess;
            message = msg;
            responseData = data;
            responseCode = response;
        }

        public bool isSuccess { get; set; }
        public string message { get; set; }
        public TData? responseData { get; set; }
        public int responseCode { get; set; }
    }
}
