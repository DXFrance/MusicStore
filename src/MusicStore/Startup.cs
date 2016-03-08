using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MusicStore.Components;
using MusicStore.Models;

namespace MusicStore
{
    public class Startup
    {
        private readonly Platform _platform;

        public Startup(IApplicationEnvironment applicationEnvironment, IRuntimeEnvironment runtimeEnvironment)
        {
            // Below code demonstrates usage of multiple configuration sources. For instance a setting say 'setting1'
            // is found in both the registered sources, then the later source will win. By this way a Local config
            // can be overridden by a different setting while deployed remotely.
            var builder = new ConfigurationBuilder()
                .SetBasePath(applicationEnvironment.ApplicationBasePath)
                .AddJsonFile("config.json")
                //All environment variables in the process's context flow in as configuration values.
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            _platform = new Platform(runtimeEnvironment);
        }

        public IConfiguration Configuration { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            var appSettings = Configuration.Get<AppSettings>("AppSettings");
            services.AddSingleton<AppSettings>(serviceProvider =>
            {
                return appSettings;
            });

            var useInMemoryStore = !_platform.IsRunningOnWindows
               || _platform.IsRunningOnMono
               || _platform.IsRunningOnNanoServer;

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

            // Add MVC services to the services container
            services.AddMvc();

            // Add memory cache services
            services.AddCaching();

            // Add session related services.
            services.AddSession();

            // Add the system clock service
            services.AddSingleton<ISystemClock, SystemClock>();

            // Configure Auth
            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "ManageStore",
                    authBuilder => {
                        authBuilder.RequireClaim("ManageStore", "Allowed");
                    });
            });
        }

        //This method is invoked when ASPNET_ENV is 'Development' or is not defined
        //The allowed values are Development,Staging and Production
        public void ConfigureDevelopment(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(minLevel: LogLevel.Warning);

            // StatusCode pages to gracefully handle status codes 400-599.
            app.UseStatusCodePagesWithRedirects("~/Home/StatusCodePage");

            // Display custom error page in production when error occurs
            // During development use the ErrorPage middleware to display error information in the browser
            app.UseDeveloperExceptionPage();

            app.UseDatabaseErrorPage();

            // Add the runtime information page that can be used by developers
            // to see what packages are used by the application
            // default path is: /runtimeinfo
            app.UseRuntimeInfoPage();

            Configure(app);
        }

        //This method is invoked when ASPNET_ENV is 'Staging'
        //The allowed values are Development,Staging and Production
        public void ConfigureStaging(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(minLevel: LogLevel.Warning);

            // StatusCode pages to gracefully handle status codes 400-599.
            app.UseStatusCodePagesWithRedirects("~/Home/StatusCodePage");

            app.UseExceptionHandler("/Home/Error");

            Configure(app);
        }

        //This method is invoked when ASPNET_ENV is 'Production'
        //The allowed values are Development,Staging and Production
        public void ConfigureProduction(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(minLevel: LogLevel.Error);

            // StatusCode pages to gracefully handle status codes 400-599.
            app.UseStatusCodePagesWithRedirects("~/Home/StatusCodePage");

            app.UseExceptionHandler("/Home/Error");

            Configure(app);
        }

        public void Configure(IApplicationBuilder app)
        {
            // Configure Session.
            app.UseSession();

            // Add static files to the request pipeline
            app.UseStaticFiles();

            // Add cookie-based authentication to the request pipeline
            app.UseIdentity();

            app.UseFacebookAuthentication(options =>
            {
                options.AppId = "550624398330273";
                options.AppSecret = "10e56a291d6b618da61b1e0dae3a8954";
            });

            app.UseGoogleAuthentication(options =>
            {
                options.ClientId = "995291875932-0rt7417v5baevqrno24kv332b7d6d30a.apps.googleusercontent.com";
                options.ClientSecret = "J_AT57H5KH_ItmMdu0r6PfXm";
            });

            app.UseTwitterAuthentication(options =>
            {
                options.ConsumerKey = "lDSPIu480ocnXYZ9DumGCDw37";
                options.ConsumerSecret = "fpo0oWRNc3vsZKlZSq1PyOSoeXlJd7NnG4Rfc94xbFXsdcc3nH";
            });

            // Add MVC to the request pipeline
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller}/{action}",
                    defaults: new { action = "Index" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });

                routes.MapRoute(
                    name: "api",
                    template: "{controller}/{id?}");
            });

            //Populates the MusicStore sample data
            SampleData.InitializeMusicStoreDatabaseAsync(app.ApplicationServices).Wait();
        }
    }
}