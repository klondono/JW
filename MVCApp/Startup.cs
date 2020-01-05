using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MVCApp
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

            //add support for OpenID Connect authentication to the MVC application
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            //adds the authentication services to DI
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
                //add the handler that can process cookies
                .AddCookie("Cookies")
                //configure the handler that perform the OpenID Connect protocol
                .AddOpenIdConnect("oidc", options =>
                {
                    //where the trusted token service is located
                    options.Authority = "http://localhost:5000";
                    options.RequireHttpsMetadata = false;

                    // identify this client via the ClientId and the ClientSecret. SaveTokens is used to persist the tokens from IdentityServer in the cookie
                    options.ClientId = "mvc";
                    options.ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0";
                    options.ResponseType = "code";
                    options.SaveTokens = true;
                });


            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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

            //ensure the authentication services execute on each request
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}")
                //The RequireAuthorization method disables anonymous access for the entire application. 
                //You can also use the [Authorize] attribute, if you want to specify that on a per controller or action method basis
                .RequireAuthorization();
            });
        }
    }
}
