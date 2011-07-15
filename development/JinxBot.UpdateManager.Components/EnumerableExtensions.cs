using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinxBot.UpdateManager.Components
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> AsEnumerated<T>(this IEnumerable<T> source)
        {
            foreach (T item in source)
                yield return item;
        }
    }
}
