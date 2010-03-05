using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinxBot.Util
{
    internal static class EnumerableExtensions
    {
        public static int IndexOf<T>(this IEnumerable<T> collection,
            T search) where T : class
        {
            int i = 0;
            foreach (T item in collection)
            {
                if (item == search)
                    return i;

                i++;
            }
            return -1;
        }
    }
}
