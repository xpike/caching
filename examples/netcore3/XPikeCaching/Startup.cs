using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XPike.Caching;
using XPike.Caching.Hybrid;
using XPike.Caching.InMemory;
using XPike.Caching.Redis;
using XPike.Configuration.Caching;
using XPike.IoC.SimpleInjector.AspNetCore;
using XPike.Logging.Microsoft.AspNetCore;
using XPike.Settings;
using XPike.Settings.AspNetCore;
using XPike.Settings.Managers;

namespace XPikeCaching
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

            services.AddXPikeSettings();

            services.AddXPikeDependencyInjection()
                .AddXPikeRedisCaching()
                .AddXPikeLogging()
                .AddXPikeConfigurationCaching()
                .RegisterSingleton(typeof(ISettings<>), typeof(SettingsLoader<>));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseXPikeDependencyInjection()
                .UseXPikeLogging()
                .UseXPikeConfigurationCaching()
                .UseXPikeCacheProvider<IHybridCachingConnectionProvider>(null)
                .AddXPikeHybridCacheProvider<IInMemoryCachingConnectionProvider>(null)
                .AddXPikeHybridCacheProvider<IRedisCachingConnectionProvider>(null);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}