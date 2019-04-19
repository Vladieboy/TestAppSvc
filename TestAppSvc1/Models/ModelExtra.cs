using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestAppSvc1.Models
{
    public class JsonResultData
    {
        public bool success { get; set; }
        public string type { get; set; }
        public object data { get; set; }
    }
}