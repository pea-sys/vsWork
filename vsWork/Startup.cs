using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using vsWork.Data;
using Npgsql;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using vsWork.Services;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace vsWork
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            // [参考]https://stackoverflow.com/questions/9218847/how-do-i-handle-database-connections-with-dapper-in-net
            services.AddSingleton<string>((sp) => this.Configuration.GetConnectionString("DefaultConnection"));
            services.AddScoped<IRepository<User,string>, UserRepository>();

            // SiganlRの接続監視(https://www.fixes.pub/program/464677.html)
            // この仕組みでユーザのログイン状況とか見ていきたい
            services.AddSingleton<UsersStateContainer>(new UsersStateContainer());
            services.AddScoped<CircuitHandler, TrackingCircuitHandler>();
            //services.AddScoped<CircuitHandler>((sp) => new CircuitHandlerService(sp.GetRequiredService<IUserOnlineService>()));
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
