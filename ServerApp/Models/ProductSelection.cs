using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Models
{
    public class ProductSelection
    {
        public long ProductId { get; set; }

        public string? Name { get; set; } = default!;

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}
