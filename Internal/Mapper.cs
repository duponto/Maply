namespace Internal
{
    internal sealed class Mapper
    {
        public static TDestination Map<TSource, TDestination>(TSource source)
            where TDestination : new()
        {
            var func = MappingCache.GetOrCreate<TSource, TDestination>();
            return func(source);
        }
    }
}