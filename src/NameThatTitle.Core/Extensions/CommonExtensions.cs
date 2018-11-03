using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace NameThatTitle.Core.Extensions
{
    public static class CommonExtensions //ToDo: replace it to separate project: 'NameThatTitle.Commons'
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return list == null || list.Count() == 0;
        }
        
        public static bool IsNotEmpty<T>(this IEnumerable<T> list)
        {
            return list != null && list.Count() != 0;
        }

        public static string ToJson<T>(this T list)
        {
            return JsonConvert.SerializeObject(list);
        }

        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
    }
}