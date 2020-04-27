using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;

namespace ServerApp.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductValuesController : Controller
    {
        readonly DataContext context;

        public ProductValuesController(DataContext ctx) => context = ctx;

        [HttpGet("{id}")]
        public Product? GetProduct(long id)
        {
            var product = context
                .Products
                .Include(p => p.Supplier).ThenInclude(s => s.Products)
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
        public IEnumerable<Product> GetProducts(string? category, string? search, bool related = false)
        {
            IQueryable<Product> serverQuery = context.Products;
            if (related)
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

                return data;
            }
            else
            {
                return clientQuery;
            }
        }
    }
}