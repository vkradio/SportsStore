using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Models.BindingTargets
{
    public class CheckoutState
    {
        public string Name { get; set; } = default!;

        public string Address { get; set; } = default!;

        public string? CardNumber { get; set; }

        public string? CardExpiry { get; set; }

        public string? CardSecurityCode { get; set; }
    }
}
