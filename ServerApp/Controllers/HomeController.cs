using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServerApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Controllers
{
    public class HomeController : Controller
    {
        readonly DataContext context;

        public HomeController(DataContext ctx) => context = ctx;

        public IActionResult Index() => View(context.Products.First());

        public IActionResult Blazor() => View();

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        [Authorize]
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Conflicts with ASP.NET MVC convention over configuration")]
        public string Protected() => "You have been authenticated";
    }
}
