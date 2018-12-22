using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using NameThatTitle.Core.Interfaces.Repositories;
using NameThatTitle.Core.Models.Localization;

namespace NameThatTitle.WebApp.Infrastructure
{
    public class DbLocalizer : IStringLocalizer
    {
        private readonly IAsyncRepository<LocalizedResource> _resourceRep;

        public DbLocalizer(IAsyncRepository<LocalizedResource> resourceRep)
        {
            _resourceRep = resourceRep;
        }

        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var format = GetString(name);
                var value = string.Format(format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return _resourceRep.All
                .Include(r => r.Culture)
                .Where(r => r.Culture.Name == CultureInfo.CurrentCulture.Name)
                .Select(r => new LocalizedString(r.Key, r.Value, true));
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            CultureInfo.DefaultThreadCurrentCulture = culture;
            return new DbLocalizer(_resourceRep);
        }

        private string GetString(string key)
        {
            return _resourceRep.All
                .Include(r => r.Culture)
                .Where(r => r.Culture.Name == CultureInfo.CurrentCulture.Name)
                .FirstOrDefault(r => r.Key == key)? //? add key to DB if not exist with 'null' value?
                .Value;
        }
    }
}
