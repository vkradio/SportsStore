using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Models.BindingTargets
{
    public class ProductData
    {
        Product product = new Product();

        [Required]
        public string? Name { get => product.Name; set => product.Name = value ?? string.Empty; }

        [Required]
        public string? Category { get => product.Category; set => product.Category = value ?? string.Empty; }

        [Required]
        public string? Description { get => product.Description; set => product.Description = value ?? string.Empty; }

        [Range(1, int.MaxValue, ErrorMessage = "Price must be at least 1")]
        public decimal Price { get => product.Price; set => product.Price = value; }

        public long? Supplier
        {
            get => product.Supplier?.SupplierId ?? null;
            
            set
            {
                if (!value.HasValue)
                {
                    product.Supplier = null;
                }
                else
                {
                    if (product.Supplier == null)
                        product.Supplier = new Supplier();
                    product.Supplier.SupplierId = value.Value;
                }
            }
        }

        public Product GetProduct() => new Product
        {
            Name = Name!,
            Category = Category!,
            Description = Description!,
            Price = Price,
            Supplier = (Supplier ?? 0) == 0 ? null : new Supplier { SupplierId = Supplier!.Value }
        };

        public void SetProduct(Product product) => this.product = product;
    }
}
