using System.Collections.Concurrent;

namespace Internal
{
    internal sealed class MappingCache
    {
        private static readonly ConcurrentDictionary<Tuple<Type, Type>, object> _cache
            = new ConcurrentDictionary<Tuple<Type, Type>, object>();

        public static Func<TSource, TDestination> GetOrCreate<TSource, TDestination>()
            where TDestination : new()
        {
            var key = new Tuple<Type, Type>(
                typeof(TSource),
                typeof(TDestination));

            object existing;

            if (_cache.TryGetValue(key, out existing))
                return (Func<TSource, TDestination>)existing;

            var created = MapBuilder.Build<TSource, TDestination>();

            _cache.TryAdd(key, created);

            return created;
        }
    }
}
