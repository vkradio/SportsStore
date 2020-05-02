using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Mvc;
using ServerApp.Models;
using ServerApp.Models.BindingTargets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Controllers
{
    [Route("api/suppliers")]
    public class SupplierValuesController : Controller
    {
        readonly DataContext context;

        public SupplierValuesController(DataContext ctx) => context = ctx;

        [HttpGet]
        public IEnumerable<Supplier> GetSuppliers() => context.Suppliers;

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
    }
}
