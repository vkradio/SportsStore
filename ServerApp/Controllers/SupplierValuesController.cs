using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;
using ServerApp.Models.BindingTargets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Controllers
{
    [Route("api/suppliers")]
    [Authorize(Roles = "Administrator")]
    public class SupplierValuesController : Controller
    {
        readonly DataContext context;

        public SupplierValuesController(DataContext ctx) => context = ctx;

        [HttpGet]
        public IEnumerable<Supplier> GetSuppliers() => context.Suppliers.AsNoTracking();

        [HttpPost]
        public IActionResult CreateSupplier([FromBody] SupplierData sdata)
        {
            if (ModelState.IsValid)
            {
                Guard.Against.Null(sdata, nameof(sdata));

                var supplier = sdata.GetSupplier();
                context.Add(supplier);
                context.SaveChanges();
                return Ok(supplier.SupplierId);
            }
            else
            {
                return BadRequest(ModelState);
            }    
        }

        [HttpPut("{id}")]
        public IActionResult ReplaceSupplier(long id, [FromBody] SupplierData sdata)
        {
            if (ModelState.IsValid)
            {
                Guard.Against.Null(sdata, nameof(sdata));

                var supplier = sdata.GetSupplier();
                supplier.SupplierId = id;
                context.Update(supplier);
                context.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpDelete("{id}")]
        public void DeleteSupplier(long id)
        {
            context.Remove(new Supplier { SupplierId = id });
            context.SaveChanges();
        }
    }
}
