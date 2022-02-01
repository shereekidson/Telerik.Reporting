using Telerik.Reporting.Application;
using Telerik.Reporting.Application.Report;
using Telerik.Reporting.Application.Services;
using Telerik.Reporting.Application.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetOpenAuth.OAuth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SecuritySettings = Telerik.Reporting.Application.Security.SecuritySettings;

namespace Telerik.Reporting.Api
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
            services.AddMvc().AddSessionStateTempDataProvider();
            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddControllers();
            services.AddControllersWithViews();
            services.AddHttpContextAccessor();
            services.AddSingleton<IConfiguration>(Configuration);

            var securitySettings = Configuration
                                    .GetSection("SecuritySettings")
                                    .Get<SecuritySettings>();

            services.AddAuthorization((options) =>
            {
                options.AddPolicy("MustBeValidUser", policyBuilder =>
                {
                    policyBuilder.RequireAuthenticatedUser();
                });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = securitySettings.TokenService;
                    options.IncludeErrorDetails = true;
                    options.Audience = $"{securitySettings.TokenService}/resources";
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            if (!(context.SecurityToken is JwtSecurityToken accessToken)) return Task.CompletedTask;

                            if (context.Principal.Identity is ClaimsIdentity identity)
                            {
                                identity.AddClaim(new Claim("access_token", accessToken.RawData));
                            }

                            return Task.CompletedTask;
                        }
                    };
                });



            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Reporting API", Version = "v1" });
            });

            ConfigureSettings(services);
            ConfigureContainer(services);

        }

        private void ConfigureContainer(IServiceCollection services)
        {

            services.AddScoped<IReportingService, ReportingService>();

        }

        private void ConfigureSettings(IServiceCollection services)
        {
            //services.Configure<BlobInfo>(Configuration.GetSection("BlobInfo"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSession();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseDefaultFiles();
            app.UseStaticFiles(); // For the wwwroot folder

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            if (env.IsDevelopment())
            {
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                // specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Reporting API");
                });

            }
        }
    }
}
