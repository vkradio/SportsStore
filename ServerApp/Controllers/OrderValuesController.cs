using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Controllers
{
    [Route("/api/orders")]
    [Authorize(Roles = "Administrator")]
    [ApiController]
    public class OrderValuesController : Controller
    {
        readonly DataContext context;

        public OrderValuesController(DataContext ctx) => context = ctx;

        decimal GetPrice(IEnumerable<CartLine> lines)
        {
            var ids = lines.Select(l => l.ProductId);

            var products = context
                .Products
                .AsNoTracking()
                .Where(p => ids.Contains(p.ProductId))
                .ToList();

            return products
                .Select(p => lines.First(l => l.ProductId == p.ProductId).Quantity * p.Price)
                .Sum();
        }

        static void ProcessPayment(Payment payment)
        {
            // integrate payment system here
            payment.AuthCode = "12345";
        }

        [HttpGet]
        public IEnumerable<Order> GetOrders() => context
            .Orders
            .AsNoTracking()
            .Include(o => o.Products)
            .Include(o => o.Payment);

        [HttpPost("{id}")]
        public void MarkShipped(long id)
        {
            var order = context.Orders.Find(id);
            if (order != null)
            {
                order.Shipped = true;
                context.SaveChanges();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult CreateOrder([FromBody] Order order)
        {
            if (ModelState.IsValid)
            {
                Guard.Against.Null(order, nameof(order));

                order.OrderId = 0;
                order.Shipped = false;
                order.Payment.Total = GetPrice(order.Products);

                ProcessPayment(order.Payment);
                if (order.Payment.AuthCode != null)
                {
                    context.Add(order);
                    context.SaveChanges();
                    return Ok(new
                    {
                        orderId = order.OrderId,
                        authCode = order.Payment.AuthCode,
                        amount = order.Payment.Total
                    });
                }
                else
                {
                    return BadRequest("Payment rejected");
                }
            }
            return BadRequest(ModelState);
        }
    }
}
