using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using WeaponStats.Models;
using WeaponStats.Services.Census;
using WeaponStats.Services.Census.Implementations;
using WeaponStats.Services.Environment;
using WeaponStats.Services.Environment.Implementations;
using WeaponStats.Services.Db;
using WeaponStats.Services.Db.Implementations;
using WeaponStats.Services.Repositories;
using WeaponStats.Services.Repositories.Implementations;
using WeaponStats.Services.Hosted;
using WeaponStats.Services.Realtime;
using WeaponStats.Services.Realtime.Implementations;

namespace WeaponStats {

    public class Startup {

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddRouting();

            services.AddCensusServices(options => {
                options.CensusServiceId = "asdf";
                options.CensusServiceNamespace = "ps2";
                options.LogCensusErrors = true;
            });

            services.AddSignalR(options => {

            });

            services.AddMvc(options => {

            }).AddRazorRuntimeCompilation();

            services.Configure<DbOptions>(Configuration.GetSection("DbOptions"));

            services.AddMemoryCache();

            services.AddSingleton<ICommandBus, CommandBus>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddSingleton<IBackgroundCharacterQueue, BackgroundCharacterQueue>();

            services.AddSingleton<IDbHelper, DbHelper>();
            services.AddSingleton<IDbCreator, DefaultDbCreator>();

            services.AddSingleton<IRealtimeMonitor, RealtimeMonitor>();
            services.AddSingleton<IRealtimeEventHandler, RealtimeEventHandler>();
            services.AddSingleton<IRealtimeEvents, RealtimeEvents>();

            // Census collection services
            services.AddSingleton<ICharacterCollection, CharacterCollection>();
            services.AddSingleton<IItemCategoryCollection, ItemCategoryCollection>();
            services.AddSingleton<ICharacterWeaponStatsCollection, CharacterWeaponStatsCollection>();
            services.AddSingleton<IOutfitCollection, OutfitCollection>();
            services.AddSingleton<IItemTypeCollection, ItemTypeCollection>();
            services.AddSingleton<IItemCollection, ItemCollection>();

            // Db store services
            services.AddSingleton<IWeaponStatDbStore, WeaponStatDbStore>();
            services.AddSingleton<ICharacterDbStore, CharacterDbStore>();
            services.AddSingleton<IItemCategoryDbStore, ItemCategoryDbStore>();
            services.AddSingleton<IItemTypeDbStore, ItemTypeDbStore>();
            services.AddSingleton<IItemDbStore, ItemDbStore>();
            services.AddSingleton<IItemStatDbStore, ItemStatDbStore>();

            // Repository services
            services.AddSingleton<ICharacterRepository, CharacterRepository>();
            services.AddSingleton<IWeaponStatRepository, WeaponStatRepository>();
            services.AddSingleton<IItemCategoryRepository, ItemCategoryRepository>();
            services.AddSingleton<IItemTypeRepository, ItemTypeRepository>();
            services.AddSingleton<IItemRepository, ItemRepository>();

            services.AddHostedService<HostedRealtimeMonitor>();
            services.AddHostedService<BackgroundCharacterUpdateHostedService>();
            services.AddHostedService<BackgroundEventProcessHostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
