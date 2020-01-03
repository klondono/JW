// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Identity.Data;
using Identity.Models;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Reflection;

namespace Identity
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            // configures IIS out-of-proc settings (see https://github.com/aspnet/AspNetCore/issues/14882)
            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            // configures IIS in-proc settings
            services.Configure<IISServerOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            //store connection string
            var connectionString = Configuration.GetConnectionString("IdentityConnection");

            //store assembly for migrations
            var migrationsAssmbly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<IdentityAppDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityAppDbContext>()
                .AddDefaultTokenProviders();


            //var builder = services.AddIdentityServer(options =>
            //{
            //    options.Events.RaiseErrorEvents = true;
            //    options.Events.RaiseInformationEvents = true;
            //    options.Events.RaiseFailureEvents = true;
            //    options.Events.RaiseSuccessEvents = true;
            //})

            var builder = services.AddIdentityServer()
                 // use sql db for storing configuration data
                 .AddConfigurationStore(configDb =>
                 {
                     configDb.ConfigureDbContext = db => db.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssmbly));
                     

                 })
                // use sql db for storing operational data
                .AddOperationalStore(operationalDb =>
                {
                    operationalDb.ConfigureDbContext = db => db.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssmbly));

                })
                //.AddInMemoryIdentityResources(Config.Ids)
                //.AddInMemoryApiResources(Config.Apis)
                //.AddInMemoryClients(Config.Clients)
                .AddAspNetIdentity<ApplicationUser>();


            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    // register your IdentityServer with Google at https://console.developers.google.com
                    // enable the Google+ API
                    // set the redirect URI to http://localhost:5000/signin-google
                    options.ClientId = "copy client ID from Google here";
                    options.ClientSecret = "copy client secret from Google here";
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            InitializeDatabase(app);

            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }

            app.UseStaticFiles();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            //using services scope
            using(var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var persistedGrantDbContext = serviceScope.ServiceProvider
                    .GetRequiredService<PersistedGrantDbContext>();
                persistedGrantDbContext.Database.Migrate();

                var configDbContext = serviceScope.ServiceProvider
                    .GetRequiredService<ConfigurationDbContext>();
                configDbContext.Database.Migrate();

                if (!configDbContext.Clients.Any())
                {
                    foreach(var client in Config.Clients)
                    {
                        configDbContext.Clients.Add(client.ToEntity());
                    }

                    configDbContext.SaveChanges();
                }

                if (!configDbContext.IdentityResources.Any())
                {
                    foreach (var res in Config.Ids)
                    {
                        configDbContext.IdentityResources.Add(res.ToEntity());
                    }

                    configDbContext.SaveChanges();
                }

                if (!configDbContext.ApiResources.Any())
                {
                    foreach (var api in Config.Apis)
                    {
                        configDbContext.ApiResources.Add(api.ToEntity());
                    }

                    configDbContext.SaveChanges();
                }
            }
        }
    }
}