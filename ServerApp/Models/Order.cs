using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Models
{
    public class Order
    {
        [BindNever]
        public long OrderId { get; set; }

        public string Name { get; set; } = default!;

        public IEnumerable<CartLine> Products { get; set; } = default!;

        public string Address { get; set; } = default!;

        public Payment Payment { get; set; } = default!;

        [BindNever]
        public bool Shipped { get; set; }
    }

    public class Payment
    {
        [BindNever]
        public long PaymentId { get; set; }

        public string CardNumber { get; set; } = default!;

        public string CardExpiry { get; set; } = default!;

        public string CardSecurityCode { get; set; } = default!;

        [BindNever]
        [Column(TypeName = "decimal(8, 2)")]
        public decimal Total { get; set; }

        [BindNever]
        public string? AuthCode { get; set; }
    }

    public class CartLine
    {
        [BindNever]
        public long CartLineId { get; set; }

        [Required]
        public long ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
