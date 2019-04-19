using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TestAppSvc1.Models;

namespace TestAppSvc1.Controllers
{
    // actually, API controller must be extended to check user token.
    public class InvoicesController : ApiController
    {
        JsonResultData obj = new JsonResultData();
        private TestDBEntities db = new TestDBEntities();

        // GET: api/Invoices
        public IEnumerable<Invoice> GetInvoices()
        {
            var data =
                (Invoice[])JsonConvert.DeserializeObject(JsonConvert.SerializeObject(db.Invoices, Utility.DTOSetting),
                    typeof(Invoice[]));
            return data;
        }

        // GET: api/Invoices/5
        [ResponseType(typeof(Invoice))]
        public async Task<IHttpActionResult> GetInvoice(string id)
        {
            //dont' insert user token check logic here
            Invoice invoice = await db.Invoices.FindAsync(id);
#if false
            obj.success = true;
            obj.type = "invoice";
            obj.data = JsonConvert.SerializeObject(invoice, Utility.DTOSetting);
#else
            var data = (Invoice)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(invoice, Utility.DTOSetting),
                typeof(Invoice));
#endif
            return Ok(data);
        }

        // PUT: api/Invoices/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutInvoice(string id, Invoice invoice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != invoice.InvoiceId)
            {
                return BadRequest();
            }

            db.Entry(invoice).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvoiceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Invoices
        [ResponseType(typeof(Invoice))]
        public IHttpActionResult PostInvoice(Invoice invoice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            invoice.InvoiceNo = Utility.GetLastInvoiceNo(db);

            db.Invoices.Add(invoice);

            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                return BadRequest(string.Join("\r\n", ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors.Select(s => s.ErrorMessage)).ToArray()));
            }
            catch (DbUpdateException ex)
            {
                if (InvoiceExists(invoice.InvoiceId))
                {
                    return Conflict();
                }
                else
                {
                    return BadRequest(ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message
                        ?? ex.Message);
                }
            }

            var data =
                (Invoice)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(invoice, Utility.DTOSetting),
                    typeof(Invoice));

            return CreatedAtRoute("DefaultApi", new { id = invoice.InvoiceId }, data);
        }

        // DELETE: api/Invoices/5
        [ResponseType(typeof(Invoice))]
        public async Task<IHttpActionResult> DeleteInvoice(string id)
        {
            Invoice invoice = await db.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            db.Invoices.Remove(invoice);
            await db.SaveChangesAsync();

            return Ok(invoice);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool InvoiceExists(string id)
        {
            return db.Invoices.Count(e => e.InvoiceId == id) > 0;
        }
    }
}