using AutoMapper;
using Common.Extensions;
using GameProcessingService.CardEffectProcessors;
using GameProcessingService.CardEffectProcessors.AutomaticallyTriggered;
using GameProcessingService.CardEffectProcessors.Played;
using GameProcessingService.CoreManagers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repository;
using Web.Hubs;
using Web.Models;

namespace Web
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
            services.AddControllers();

            services.AddSingleton<IGameManager, GameManager>();
            services.AddSingleton<IPlayCardManager, PlayCardManager>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IGameRepository, GameRepository>();
            services.AddSingleton<ITournamentRepository, TournamentRepository>();
            services.AddSingleton<ITournamentManager, TournamentManager>();
            services.AddSingleton<IHallOfFameRepository, HallOfFameRepository>();
            services.RegisterAllTypes<IPlayedCardEffectProcessor>(new[] { typeof(IPlayedCardEffectProcessor).Assembly }, ServiceLifetime.Singleton);
            services.RegisterAllTypes<IAutomaticallyTriggeredCardEffectProcessor>(new[] { typeof(IAutomaticallyTriggeredCardEffectProcessor).Assembly }, ServiceLifetime.Singleton);
            services.Configure<AppSettings>(Configuration.GetSection("UnoAppSettings"));


            services.AddSignalR();
            services.AddAutoMapper(typeof(Startup));

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
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
            }

            app.UseRouting();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<GameHub>("/gamehub");
                endpoints.MapControllerRoute("default", "{controller}/{action=Index}/{id?}");
            });



            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
