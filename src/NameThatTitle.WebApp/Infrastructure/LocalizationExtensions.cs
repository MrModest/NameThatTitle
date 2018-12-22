using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using NameThatTitle.Core.Static;

namespace NameThatTitle.WebApp.Infrastructure
{
    public static class LocalizationExtensions //ToDo: replace it to separate project: 'NameThatTitle.Commons'
    {
        //ToDo: Change to DbTable. Also add Controllers for Web GUI for add translated string (it's let add localization with community)
        public static IServiceCollection AddDbLocalization(this IServiceCollection services, Action<DbLocalizationOptions> setupAction = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAdd(new ServiceDescriptor(
                typeof(IStringLocalizerFactory),
                typeof(DbLocalizerFactory),
                ServiceLifetime.Singleton));
            services.TryAdd(new ServiceDescriptor(
                typeof(IStringLocalizer),
                typeof(DbLocalizer),
                ServiceLifetime.Singleton));

            if (setupAction != null)
            {
                services.Configure(setupAction);
            }

            //services.AddLocalization(options => options.ResourcesPath = "Localizations");

            return services;
        }

        //ToDo: must not be run for getting static resources like js/css. Only controllers?
        public static IServiceCollection ConfigureRequestLocalization(this IServiceCollection services)
        {
            services.Configure<RequestLocalizationOptions>(options => 
            {
                var supportCultures = new[] { StaticData.Culture.En, StaticData.Culture.Ru };

                options.DefaultRequestCulture = new RequestCulture(supportCultures[0]);

                options.SupportedCultures = supportCultures;
                options.SupportedUICultures = supportCultures;

                //0 - QueryStringRequestCultureProvider
                //1 - UserPreferenceRequestCultureProvider
                //2 - CookieRequestCultureProvider
                //3 - AcceptLanguageHeaderRequestCultureProvider
                options.RequestCultureProviders.Insert(1, new UserPreferenceRequestCultureProvider());
            });

            return services;
        }
    }
}
