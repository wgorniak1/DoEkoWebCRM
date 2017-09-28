using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DoEko.Models.Identity;
using DoEko.Services;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;
using DoEko.Models.DoEko;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using DoEko.Controllers.Settings;
using Microsoft.Extensions.Options;
using Microsoft.AspNet.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;

namespace DoEko
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //
            services.AddAuthorization();
            //
            services.AddSingleton<IConfiguration>(Configuration);

            //DB connections
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<DoEkoContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            //
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            //
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                
                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.AllowedForNewUsers = true;

                // Cookie settings
                options.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromMinutes(2);
                options.Cookies.ApplicationCookie.LoginPath = "/Account/LogIn";
                options.Cookies.ApplicationCookie.LogoutPath = "/Account/LogOut";

                // User settings
                options.User.RequireUniqueEmail = true;
            });


            //Authorization & authentication
            AuthorizationPolicy requireAuthenticatedUser = new 
                AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

            services.AddMvc();// options =>
            //{
              //  options.Filters.Add(new AuthorizeFilter(requireAuthenticatedUser));
                //options.RequireHttpsPermanent = true;
                //options.ModelBinderProviders.Insert(0,new Models.DoubleModelBinderProvider());
                //options.OutputFormatters.Insert(0, new Models.DoubleFormatProvider());
            //});

            services.Configure<MvcOptions>(options => 
            {
                options.Filters.Add(new AuthorizeFilter(requireAuthenticatedUser));
                options.Filters.Add(new RequireHttpsAttribute());
            });

            services.AddCors();

            //Session
            services.AddMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.CookieName = ".DoEko";
            });

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            services.AddTransient<IFileStorage, AzureStorage>();

            //Options mapped to class
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            //
            services.AddLogging();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(Microsoft.AspNetCore.Builder.IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptions<AppSettings> settings)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseRequestLocalization(new RequestLocalizationOptions
            {

                DefaultRequestCulture = new RequestCulture("en-GB"),
                // Formatting numbers, dates, etc.
                SupportedCultures = new List<CultureInfo>() { new CultureInfo("en-GB") },
                // UI strings that we have localized.
                SupportedUICultures = new List<CultureInfo>() { new CultureInfo("en-GB"), new CultureInfo("pl-PL") },
            });

            ////
            app.UseStaticFiles();
            app.UseIdentity();
            app.UseSession();

            //
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "Cookie",
                LoginPath = new PathString("/Account/Login/"),
                AccessDeniedPath = new PathString("/Account/Forbidden/"),
                AutomaticAuthenticate = true,
                AutomaticChallenge = false,
                CookieName = "TokenAuth",
                LogoutPath="/Account/Logout",
                SlidingExpiration = true,
                ExpireTimeSpan = TimeSpan.FromMinutes(2)
            });

            //app.UseCookieAuthentication(new CookieAuthenticationOptions
            //{
            //    AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
            //    LoginPath = new PathString("/Account/Login"),
            //    Provider = new CookieAuthenticationProvider
            //    {
            //        OnApplyRedirect = ctx =>
            //        {
            //            if (!IsAjaxRequest(ctx.Request))
            //            {
            //                ctx.Response.Redirect(ctx.RedirectUri);
            //            }
            //        }
            //    }
            //});

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseCors(b => b.AllowAnyHeader());
            
            //force https
            var options = new RewriteOptions()
                .AddRedirectToHttps();

            app.UseRewriter(options);

            // Seed Address initial catalog
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetService<DoEkoContext>().Database.Migrate();
                serviceScope.ServiceProvider.GetService<ApplicationDbContext>().Database.Migrate();

                serviceScope.ServiceProvider.GetService<DoEkoContext>().EnsureSeedData();
            }

            // Seed initial roles & admin
            app.EnsureRolesCreated(settings);

        }
    }
}
