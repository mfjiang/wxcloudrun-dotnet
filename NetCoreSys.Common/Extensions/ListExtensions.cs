using System;
using System.Collections;
using System.Collections.Generic;

namespace NetCoreSys.Common.Extensions
{
    public static class ListExtensions
    {
        public static void AddRange(this IList list, IEnumerable enumerable)
        {
            foreach (var item in enumerable) list.Add(item);
        }

        public static void Foreach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list) action(item);
        }
    }
}