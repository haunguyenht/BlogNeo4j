using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using BlogApplication.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BlogApplication.Models;
using Neo4jClient;
using Microsoft.Extensions.Options;
using BlogApplication.Repository;
using Neo4jClient.Transactions;
using System.Transactions;
using Neo4j.Driver.V1;

namespace BlogApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            RegisterGraphDriver(services);
            RegisterGraphClient(services);
            RegisterRepositories(services);

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        private IServiceCollection RegisterGraphClient(IServiceCollection services)
        {
            services.AddSingleton(typeof(IBoltGraphClient), resolver =>
            {
                var client = new BoltGraphClient(services.BuildServiceProvider().GetService<IDriver>());

                if (!client.IsConnected)
                {
                    client.Connect();
                }

                return client;
            });

            return services;
        }

        private IServiceCollection RegisterGraphDriver(IServiceCollection services)
        {
            services.AddSingleton(typeof(IDriver), resolver =>
            {
                var neo4jSetting = Configuration.GetSection("Neo4jSetting").Get<Neo4jDbSettings>();
                var authToken = AuthTokens.Basic(neo4jSetting.Username, neo4jSetting.Password);
                var config = new Config();
                var driver = GraphDatabase.Driver(neo4jSetting.Uri, authToken, config);
                return driver;
            });

            return services;
        }

        private IServiceCollection RegisterRepositories(IServiceCollection services)
        {
            services.AddTransient(typeof(IRepository), typeof(Neo4jRepository));
            return services;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider servicesProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapDefaultControllerRoute();
            });

            SeedData.SeedData.InitializeAsync(servicesProvider).Wait();

        }
    }
}
