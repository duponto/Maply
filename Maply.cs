using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Maply;

public class Mapper
{
    private static readonly ConcurrentDictionary<(Type Source, Type Destination), Delegate> Cache = new();

    public static TDestination Map<TDestination>(object source)
        where TDestination : new()
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        var sourceType = source.GetType();
        var key = (sourceType, typeof(TDestination));

        var mapper = (Func<object, TDestination>)Cache.GetOrAdd(
            key,
            _ => CreateMap<TDestination>(sourceType));

        return mapper(source);
    }

    private static Func<object, TDestination> CreateMap<TDestination>(Type sourceType)
        where TDestination : new()
    {
        var sourceParam = Expression.Parameter(typeof(object), "source");
        var typedSource = Expression.Convert(sourceParam, sourceType);
        var bindings = new List<MemberBinding>();

        var sourceProperties = sourceType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var destinationProperties = typeof(TDestination)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var destProp in destinationProperties)
        {
            if (!destProp.CanWrite)
                continue;

            var sourceProp = sourceProperties.FirstOrDefault(p =>
                p.CanRead &&
                p.Name == destProp.Name &&
                p.PropertyType == destProp.PropertyType);

            if (sourceProp is null)
                continue;

            var sourcePropertyAccess = Expression.Property(typedSource, sourceProp);

            var bind = Expression.Bind(destProp, sourcePropertyAccess);
            bindings.Add(bind);
        }

        var body = Expression.MemberInit(
            Expression.New(typeof(TDestination)),
            bindings);

        var lambda = Expression.Lambda<Func<object, TDestination>>(body, sourceParam);

        return lambda.Compile();
    }
}
