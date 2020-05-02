using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Models
{
    public class Product
    {
        public long ProductId { get; set; }

        public string Name { get; set; } = default!;
        public string Category { get; set; } = default!;
        public string Description { get; set; } = default!;

        [Column(TypeName = "decimal(8, 2)")]
        public decimal Price { get; set; }

        public Supplier? Supplier { get; set; }
#pragma warning disable CA2227 // Collection properties should be read only
        public List<Rating> Ratings { get; set; } = default!;
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
