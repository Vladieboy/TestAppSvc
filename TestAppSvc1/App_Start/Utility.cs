using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using TestAppSvc1.Models;
using Newtonsoft.Json;

namespace TestAppSvc1
{
    public class Utility
    {

        public static string GetLastInvoiceNo(TestDBEntities db)
        {
            var ivno = db.Invoices.OrderByDescending(x => x.InvoiceNo).Select(x => x.InvoiceNo).FirstOrDefault();
            ivno = ivno ?? "10000";
            int _no = 0;
            int.TryParse(ivno, out _no);
            return (++_no).ToString();
        }

        public static JsonSerializerSettings DTOSetting
        {
            get
            {
                return new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Error = (sender, args) =>
                    {
                        args.ErrorContext.Handled = true;
                    },
                };
            }
        }
    }
}