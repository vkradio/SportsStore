using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ServerApp.Models;
using ServerApp.Models.BindingTargets;

namespace ServerApp.Controllers
{
    [Route("api/products")]
    [ApiController]
    [Authorize(Roles = "Administrator")]
    public class ProductValuesController : Controller
    {
        readonly DataContext context;

        public ProductValuesController(DataContext ctx) => context = ctx;

        IActionResult CreateMetadata(IEnumerable<Product> products) => Ok(new
        {
            data = products,
            categories = context
                .Products
                .AsNoTracking()
                .Select(p => p.Category)
                .Distinct()
                .OrderBy(c => c)
        });

        [HttpGet("{id}")]
        [AllowAnonymous]
        public Product? GetProduct(long id)
        {
            var product = context
                .Products
                .Include(p => p.Supplier!) // See https://docs.microsoft.com/en-us/ef/core/miscellaneous/nullable-reference-types#navigating-and-including-nullable-relationships
                    .ThenInclude(s => s.Products)
                .Include(p => p.Ratings)
                .FirstOrDefault(p => p.ProductId == id);

            if (product != null)
            {
                if (product.Supplier != null)
                {
                    product.Supplier.Products = product
                        .Supplier
                        .Products
                        .Select(p => new Product
                        {
                            ProductId = p.ProductId,
                            Name = p.Name,
                            Category = p.Category,
                            Description = p.Description,
                            Price = p.Price
                        });
                }

                if (product.Ratings != null)
                {
                    foreach (var r in product.Ratings)
                        r.Product = null!;
                }
            }

            return product;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetProducts(string? category, string? search, bool related = false, bool metadata = false)
        {
            IQueryable<Product> serverQuery = context.Products.AsNoTracking();
            if (related && HttpContext.User.IsInRole("Administrator"))
            {
                serverQuery = serverQuery
                    .Include(p => p.Supplier)
                    .Include(p => p.Ratings);
            }
            var clientQuery = serverQuery.AsEnumerable(); // Read to client's memory because of too complex expressions in textbook

            if (!string.IsNullOrWhiteSpace(category))
            {
                var catUpper = category.ToUpperInvariant();
                clientQuery = clientQuery.Where(p => p.Category.ToUpperInvariant().Contains(catUpper, StringComparison.InvariantCulture));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchUpper = search.ToUpperInvariant();
                clientQuery = clientQuery.Where(p => p.Name.ToUpperInvariant().Contains(searchUpper, StringComparison.InvariantCulture) ||
                                                     p.Description.ToUpperInvariant().Contains(searchUpper, StringComparison.InvariantCulture));
            }

            if (related)
            {
                var data = clientQuery.ToList();

                data
                    .ForEach(p =>
                    {
                        if (p.Supplier != null)
                            p.Supplier.Products = null!;
                        if (p.Ratings != null)
                            p.Ratings.ForEach(r => r.Product = null!);
                    });

                return metadata ? CreateMetadata(data) : Ok(data);
            }
            else
            {
                return metadata ? CreateMetadata(clientQuery) : Ok(clientQuery);
            }
        }

        [HttpPost]
        public IActionResult CreateProduct([FromBody] ProductData pData)
        {
            if (ModelState.IsValid)
            {
                Guard.Against.Null(pData, nameof(pData));

                var product = pData.GetProduct();

                if (product.Supplier != null && product.Supplier.SupplierId != 0)
                    context.Attach(product.Supplier);

                context.Add(product);

                context.SaveChanges();

                return Ok(product.ProductId);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut("{id}")]
        public IActionResult ReplaceProduct(long id, [FromBody] ProductData pdata)
        {
            if (ModelState.IsValid)
            {
                Guard.Against.Null(pdata, nameof(pdata));

                var product = pdata.GetProduct();
                product.ProductId = id;
                if (product.Supplier != null && product.Supplier.SupplierId != 0)
                    context.Attach(product.Supplier);
                context.Update(product);
                context.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("{id}")]
        public IActionResult UpdateProduct(long id, [FromBody] JsonPatchDocument<ProductData> patch)
        {
            var product = context
                .Products
                .Include(p => p.Supplier)
                .First(p => p.ProductId == id);
            var pdata = new ProductData();
            pdata.SetProduct(product);

            patch.ApplyTo(pdata, ModelState);

            if (ModelState.IsValid && TryValidateModel(pdata))
            {
                if (product.Supplier != null && product.Supplier.SupplierId != 0)
                    context.Attach(product.Supplier);
                context.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpDelete("{id}")]
        public void DeleteProduct(long id)
        {
            context.Products.Remove(new Product { ProductId = id });
            context.SaveChanges();
        }
    }
}
