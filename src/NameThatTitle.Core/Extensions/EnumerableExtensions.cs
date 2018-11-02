using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace NameThatTitle.Core.Extensions
{
    public static class EnumerableExtensions //ToDo: replace it to separate project: 'NameThatTitle.Commons'
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return list == null || !list.Any();
        }
        
        public static bool IsNotEmpty<T>(this IEnumerable<T> list)
        {
            return list != null && list.Any();
        }

        public static string ToJson<T>(this IEnumerable<T> list)
        {
            return JsonConvert.SerializeObject(list);
        }
    }
}