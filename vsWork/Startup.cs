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
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Fluxor;
using Blazored.Toast;
using Blazored.LocalStorage;
using Blazored.Modal;
using vsWork.Stores;

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

            services.AddBlazorise(options =>
            {
                options.ChangeTextOnKeyPress = true;
            })
            .AddBootstrapProviders()
            .AddFontAwesomeIcons();

            //サービスのライフタイムについて
            //https://entityframeworkcore.com/knowledge-base/57495333/asp-net-core---repository-dependency-injection-fails-on-singleton-injection

            // [参考]https://stackoverflow.com/questions/9218847/how-do-i-handle-database-connections-with-dapper-in-net
            services.AddSingleton<string>((sp) => this.Configuration.GetConnectionString("DefaultConnection"));
            services.AddScoped<IRepository<User, string>, UserRepository>();
            services.AddScoped<IRepository<Session, string>, SessionRepository>();
            services.AddScoped<IRepository<Attendance, string>, AttendanceRepository>();
            services.AddScoped<IRepository<UserState, string>, UserStateRepository>();
            services.AddScoped<IRepository<Organization, string>, OrganizationRepository>();

            // SignalRクライアントの一意の回線ID状況を監視(Scopedにすることでクライアント毎に一意の回線IDを取得可能)
            // [参考]https://www.fixes.pub/program/464677.html
            services.AddScoped<CircuitHandler, CircuitHandlerService>((sp) => new CircuitHandlerService(sp.GetRequiredService<IDispatcher>()));




            services.AddFluxor(options => options.ScanAssemblies(typeof(Startup).Assembly));
            services.AddBlazoredToast();

            services.AddBlazoredLocalStorage(config =>
            {
                config.JsonSerializerOptions.WriteIndented = true;
            });
            services.AddBlazoredModal();
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
