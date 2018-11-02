using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using NameThatTitle.Core.Static;

namespace NameThatTitle.Core.Utils
{
    public static class LocalizationUtils //ToDo: replace it to separate project: 'NameThatTitle.Commons'
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
