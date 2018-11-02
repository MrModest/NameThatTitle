using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NameThatTitle.Data.DbContexts;
using NameThatTitle.Data.Repositories;
using NameThatTitle.Core.Interfaces.Repositories;
using NameThatTitle.Core.Interfaces.Services;
using NameThatTitle.Core.Models.Users;
using NameThatTitle.Core.Services;
using NameThatTitle.Core.Static;
using NameThatTitle.Core.Utils;
using NameThatTitle.WebApp.Infrastructure;

namespace NameThatTitle.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppIdentityContext>(options =>
                options.UseNpgsql(
                    Configuration["ConnectionStrings:AppIdentity"],
                    npgsqlOptions => npgsqlOptions.MigrationsAssembly("NameThatTitle.Data")));

            services.AddDbContext<ForumContext>(option =>
                option.UseNpgsql(Configuration["ConnectionStrings:Forum"]));

            services.AddIdentity<UserAccount, UserRole>()
                .AddErrorDescriber<MultiLanguageIdentityErrorDescriber>()
                .AddEntityFrameworkStores<AppIdentityContext>();

            services.Configure<IdentityOptions>(options =>
            {
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";
                options.User.RequireUniqueEmail = true;

                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;

                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 7;
                options.Password.RequiredUniqueChars = 2;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });

            //? Is it works for SPA?
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            services.AddTransient(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddTransient(typeof(IAsyncRepository<>), typeof(EfRepository<>));
            services.AddTransient<IAsyncRefreshTokenRepository, RefreshTokenRepository>();
            services.AddTransient<ITokenHandler, JwtHandler>();
            services.AddTransient<IAccountService, AccountService>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        //ValidIssuer = "",

                        ValidateAudience = false,
                        //ValidAudience = "",

                        ValidateLifetime = true,

                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Token:Key"])),
                        ValidateIssuerSigningKey = true
                    };
                });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

            //ToDo: Change to DbTable. Also add Controllers for Web GUI for add translated string (it's let add localization with community)
            services.AddLocalization(options => options.ResourcesPath = "Localizations");

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddDataAnnotationsLocalization();

            services.Configure<RequestLocalizationOptions>(LocalizationUtils.ConfigureRequestLocalizationOptions);

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error"); //ToDo: throw custom exception from methods/services/controllers than catch him and response BadRequest with error json
                app.UseHsts();
            }

            app.UseRequestLocalization(); //? Need argument?

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseAuthentication();

            app.UseMiddleware<ErrorHandler.ErrorHandlerMiddleware>(); //ToDo: replace to Extension: 'app.UseErrorHandler()'

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}