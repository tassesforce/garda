using System;
using System.Diagnostics;
using garda.Models.Context.ClientContext;
using garda.Models.Context.RoleContext;
using garda.Models.Context.TokenContext;
using garda.Models.Context.UserAuthContext;
using lug.Middleware.Request;
using lug.Middleware.Log;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Serilog;
using lug.Middleware.Correlation;
using garda.Exception.Filter;
using garda.Services.Historisation;

namespace garda
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
            services.AddDbContext<UserAuthDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<HistoUserAuthDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<ClientAppDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<RoleDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<RevokedTokenDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
                
            services.AddLogging(loggingBuilder =>
            loggingBuilder.AddSerilog(dispose: true));
            services.AddMvc(
                config => { 
                    config.Filters.Add(typeof(CustomExceptionFilter));
                }
            ).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseMiddleware<SerilogMiddleware>();
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
            app.UseMiddleware<CorrelationIdMiddleware>();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
