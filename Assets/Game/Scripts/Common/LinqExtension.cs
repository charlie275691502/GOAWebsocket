using OneOf.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.LinqExtension
{
	public static class UniTaskExtension
    {
        public static void ZipForEach<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second, Action<T1, T2> action)
            => first
                .Zip(second, (a, b) =>
                {
                    action(a, b);
                    return default(None);
                })
                .ToArray();

        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            source
                .Select((obj, index) => (obj, index))
                .ToList()
                .ForEach(tuple => action?.Invoke(tuple.obj, tuple.index));
        }
    }
}
