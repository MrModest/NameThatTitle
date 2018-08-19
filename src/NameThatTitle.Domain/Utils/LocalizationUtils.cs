using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using NameThatTitle.Domain.Static;

namespace NameThatTitle.Domain.Utils
{
    public static class LocalizationUtils
    {
        public static void ConfigureRequestLocalizationOptions(RequestLocalizationOptions options)
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
        }
    }
}
