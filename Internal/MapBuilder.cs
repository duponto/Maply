using System.Linq.Expressions;
using System.Reflection;
namespace TypeBridge.Mapping
{
    internal sealed class MapBuilder
    {
        public static Func<TSource, TDestination> Build<TSource, TDestination>()
            where TDestination : new()
        {
            Type sourceType = typeof(TSource);
            Type destType = typeof(TDestination);

            ParameterExpression sourceParameter =
                Expression.Parameter(sourceType, "source");

            List<MemberBinding> bindings = new List<MemberBinding>();

            PropertyInfo[] sourceProperties =
                sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            PropertyInfo[] destProperties =
                destType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo destProp in destProperties)
            {
                if (!destProp.CanWrite)
                    continue;

                PropertyInfo sourceProp = sourceProperties
                    .FirstOrDefault(p =>
                        p.CanRead &&
                        p.Name == destProp.Name &&
                        p.PropertyType == destProp.PropertyType);

                if (sourceProp == null)
                    continue;

                MemberExpression sourceAccess =
                    Expression.Property(sourceParameter, sourceProp);

                MemberBinding binding =
                    Expression.Bind(destProp, sourceAccess);

                bindings.Add(binding);
            }

            MemberInitExpression body =
                Expression.MemberInit(
                    Expression.New(destType),
                    bindings);

            Expression<Func<TSource, TDestination>> lambda =
                Expression.Lambda<Func<TSource, TDestination>>(
                    body,
                    sourceParameter);

            return lambda.Compile();
        }
    }
}