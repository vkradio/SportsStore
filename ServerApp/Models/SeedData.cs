using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Models
{
    public static class SeedData
    {
        public static void SeedDatabase(DataContext context)
        {
            Guard.Against.Null(context, nameof(context));

            context.Database.Migrate();

            if (context.Products.Any())
                return;

            var supplier1 = new Supplier { Name = "Splash Dudes", City = "San Jose", State = "CA" };
            var supplier2 = new Supplier { Name = "Soccer Town", City = "Chicago", State = "IL" };
            var supplier3 = new Supplier { Name = "Chess Co", City = "New York", State = "NY" };

            context.Products.AddRange(
                new Product
                {
                    Name = "Kayak",
                    Description = "A boat for one person",
                    Category = "Watersports",
                    Price = 275,
                    Supplier = supplier1,
                    Ratings = new List<Rating>
                    {
                        new Rating { Stars = 4 },
                        new Rating { Stars = 3 }
                    }
                },
                new Product
                {
                    Name = "Lifejacket",
                    Description = "Protective and fashionable",
                    Category = "Watersports",
                    Price = 48.95m,
                    Supplier = supplier1,
                    Ratings = new List<Rating>
                    {
                        new Rating { Stars = 2 },
                        new Rating { Stars = 5 }
                    }
                },
                new Product
                {
                    Name = "Soccer Ball",
                    Description = "FIFA-approved size and weight",
                    Category = "Soccer",
                    Price = 19.50m,
                    Supplier = supplier2,
                    Ratings = new List<Rating>
                    {
                        new Rating { Stars = 1 },
                        new Rating { Stars = 3 }
                    }
                },
                new Product
                {
                    Name = "Corner Flags",
                    Description = "Give your pitch a professional touch",
                    Category = "Soccer",
                    Price = 34.95m,
                    Supplier = supplier2,
                    Ratings = new List<Rating>
                    {
                        new Rating { Stars = 3 }
                    }
                },
                new Product
                {
                    Name = "Stadium",
                    Description = "Flat-packed 35,000-seat stadium",
                    Category = "Soccer",
                    Price = 79500,
                    Supplier = supplier2,
                    Ratings = new List<Rating>
                    {
                        new Rating { Stars = 1 },
                        new Rating { Stars = 4 },
                        new Rating { Stars = 3 }
                    }
                },
                new Product
                {
                    Name = "Thinking Cap",
                    Description = "Improve brain efficiency by 75%",
                    Category = "Chess",
                    Price = 16,
                    Supplier = supplier3,
                    Ratings = new List<Rating>
                    {
                        new Rating { Stars = 5 },
                        new Rating { Stars = 4 }
                    }
                },
                new Product
                {
                    Name = "Unsteady Chair",
                    Description = "Secretly give your opponent a disadvantage",
                    Category = "Chess",
                    Price = 29.95m,
                    Supplier = supplier3,
                    Ratings = new List<Rating>
                    {
                        new Rating { Stars = 3 }
                    }
                },
                new Product
                {
                    Name = "Human Chess Board",
                    Description = "A fun game for the family",
                    Category = "Chess",
                    Price = 75,
                    Supplier = supplier3
                },
                new Product
                {
                    Name = "Bling-Bling King",
                    Description = "Gold-plated, diamond-studded King",
                    Category = "Chess",
                    Price = 1200,
                    Supplier = supplier3
                }
            );

            context.SaveChanges();
        }
    }
}
