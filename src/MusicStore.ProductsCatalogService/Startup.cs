using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MusicStore.Models;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MusicStore.ProductsCatalogService
{
    public class Startup
    {
        private readonly Platform _platform;
        
        public Startup(IHostingEnvironment env, IRuntimeEnvironment runtimeEnvironment)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            
            _platform = new Platform(runtimeEnvironment);
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            // var useInMemoryStore = !_platform.IsRunningOnWindows
            //    || _platform.IsRunningOnMono
            //    || _platform.IsRunningOnNanoServer;
            var useInMemoryStore = true;

            // Add EF services to the services container
            if (useInMemoryStore)
            {
               services.AddEntityFramework()
                       .AddInMemoryDatabase()
                       .AddDbContext<MusicStoreContext>(options =>
                           options.UseInMemoryDatabase());
            }
            else
            {
               services.AddEntityFramework()
                       .AddSqlServer()
                       .AddDbContext<MusicStoreContext>(options =>
                           options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));
            }
            
            // Add Identity services to the services container
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                    {
                        options.Cookies.ApplicationCookie.AccessDeniedPath = "/Home/AccessDenied";
                    })
                    .AddEntityFrameworkStores<MusicStoreContext>()
                    .AddDefaultTokenProviders();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.WithOrigins("http://example.com");
                });
            });

            // Add memory cache services
            services.AddCaching();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseStaticFiles();

            app.UseMvc();
            
            //Populates the MusicStore sample data
            SampleData.InitializeMusicStoreDatabaseAsync(app.ApplicationServices).Wait();
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
