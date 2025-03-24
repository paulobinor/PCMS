using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pcms.Application
{
    public class ApiResponse<T> 
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public T? Data { get; set; }
    }
}
