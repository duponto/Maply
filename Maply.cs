using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Maply;

public static class Mapper
{
    private static readonly ConcurrentDictionary<(Type Source, Type Destination), Delegate> Cache = new();

    public static TDestination Map<TSource, TDestination>(TSource source)
        where TDestination : new()
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        var key = (typeof(TSource), typeof(TDestination));

        var mapper = (Func<TSource, TDestination>)Cache.GetOrAdd(key, _ => CreateMap<TSource, TDestination>());

        return mapper(source);
    }

    private static Func<TSource, TDestination> CreateMap<TSource, TDestination>()
        where TDestination : new()
    {
        var sourceParam = Expression.Parameter(typeof(TSource), "source");
        var bindings = new List<MemberBinding>();

        var sourceProperties = typeof(TSource)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var destinationProperties = typeof(TDestination)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite);

        foreach (var destProp in destinationProperties)
        {
            var sourceProp = sourceProperties.FirstOrDefault(p =>
                p.Name == destProp.Name &&
                p.PropertyType == destProp.PropertyType);

            if (sourceProp is null)
                continue;

            var sourcePropertyAccess = Expression.Property(sourceParam, sourceProp);

            var bind = Expression.Bind(destProp, sourcePropertyAccess);
            bindings.Add(bind);
        }

        var body = Expression.MemberInit(
            Expression.New(typeof(TDestination)),
            bindings);

        var lambda = Expression.Lambda<Func<TSource, TDestination>>(body, sourceParam);

        return lambda.Compile();
    }
}
