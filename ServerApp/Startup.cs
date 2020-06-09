using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using ServerApp.Models;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace ServerApp
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            Guard.Against.Null(env, nameof(env));

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .AddCommandLine(Environment
                    .GetCommandLineArgs()
                    .Skip(1)
                    .ToArray()
                );

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Not compatible with ASP.NET MVC convention over configuration principle")]
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration["ConnectionStrings:DefaultConnection"];

            services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));

            services.AddDbContext<IdentityDataContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:Identity"]));
            services
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDataContext>();

            services
                .AddControllersWithViews()
                .AddJsonOptions(opts => { opts.JsonSerializerOptions.IgnoreNullValues = true; })
                .AddNewtonsoftJson();

            //services.AddSwaggerGen(options =>
            //{
            //    options.SwaggerDoc("v1", new OpenApiInfo { Title = "SportsStore API", Version = "v1" });
            //});

            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = connectionString;
                options.SchemaName = "dbo";
                options.TableName = "SessionData";
            });

            services.AddSession(options =>
            {
                options.Cookie.Name = "SportsStore.Session";
                options.IdleTimeout = TimeSpan.FromHours(48);
                options.Cookie.HttpOnly = false;
                options.Cookie.IsEssential = true;
            });

            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "It will overcomplicate if we'll put those 2 strings into resources")]
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IServiceProvider services,
            IAntiforgery antiforgery,
            IHostApplicationLifetime lifetime)
        {
            Guard.Against.Null(app, nameof(app));
            Guard.Against.Null(lifetime, nameof(lifetime));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = "/blazor",
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "../BlazorApp/wwwroot"))
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = "",
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "./wwwroot/app"))
            });

            app.UseSession();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.Use(nextDelegate => context =>
            {
                var path = context.Request.Path.Value;
                string[] directUrls = { "/admin", "/store", "/cart", "checkout" };
                if (path.StartsWith("/api", StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals("/", path, StringComparison.InvariantCultureIgnoreCase) ||
                    directUrls.Any(url => path.StartsWith(url, StringComparison.InvariantCultureIgnoreCase)))
                {
                    var tokens = antiforgery.GetAndStoreTokens(context);
                    context.Response.Cookies.Append(
                        "XSRF-TOKEN",
                        tokens.RequestToken,
                        new CookieOptions
                        {
                            HttpOnly = false,
                            Secure = false,
                            IsEssential = true
                        }
                    );
                }
                return nextDelegate(context);
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );

                endpoints.MapControllerRoute(
                    name: "angular_fallback",
                    pattern: "{target:regex(admin|store|cart|checkout):nonfile}/{*catchall}",
                    defaults: new { controller = "Home", action = "Index" }
                );

                endpoints.MapControllerRoute(
                    name: "blazor_integration",
                    pattern: "/blazor/{*path:nonfile}",
                    defaults: new { controller = "Home", action = "Blazor" }
                );

                //endpoints.MapFallbackToFile("blazor/{*path:nonfile}", "index.html");
            });

            //app.Map("/blazor", opts => opts.UseBlazorFrameworkFiles());
            app.UseBlazorFrameworkFiles();

            //app.UseSwagger();
            //app.UseSwaggerUI(options =>
            //{
            //    options.SwaggerEndpoint("/swagger/v1/swagger.json", "SportsStore API");
            //});

            //app.UseSpa(spa =>
            //{
            //    var strategy = Configuration.GetValue<string>("DevTools:ConnectionStrategy");
            //    if (strategy == "proxy")
            //    {
            //        spa.UseProxyToSpaDevelopmentServer("http://127.0.0.1:4200");
            //    }
            //    else
            //    {
            //        spa.Options.SourcePath = "../ClientApp";
            //        spa.UseAngularCliServer("start");
            //    }
            //});

            if ((Configuration["INITDB"] ?? "false") == "true")
            {
                Console.WriteLine("Preparing Database...");
                SeedData.SeedDatabase(services.GetRequiredService<DataContext>());
                IdentitySeedData.SeedDatabase(services).Wait();
                Console.WriteLine("Database Preparation Complete");
                lifetime.StopApplication();
            }
        }
    }
}
