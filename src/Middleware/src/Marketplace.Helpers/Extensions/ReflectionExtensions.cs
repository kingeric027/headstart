using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Marketplace.Helpers.Extensions
{
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Get a PropertyInfo from a lambda
        /// </summary>
        public static PropertyInfo GetPropertyInfo<T, TProp>(this Expression<Func<T, TProp>> property)
        {
            // http://stackoverflow.com/a/672212/62600

            var member = (property.Body.NodeType == ExpressionType.Convert) ?
                ((UnaryExpression)property.Body).Operand as MemberExpression :
                property.Body as MemberExpression;

            if (member == null)
                throw new Exception($"Can't get property info for {property} because it's not a property.");

            var pi = member.Member as PropertyInfo;
            if (pi == null)
                throw new Exception($"Can't get property info for {property} because it's not a property.");

            return pi;
        }

        public static bool HasAttribute<T>(this Type type, bool includeInherited = true) where T : Attribute
        {
            var result = type.GetAttributes<T>(includeInherited);
            return result.Any();
        }

        public static bool HasAttribute<T>(this MemberInfo member, bool includeInherited = true) where T : Attribute
        {
            var result = member.GetAttributes<T>(includeInherited);
            return result.Any();
        }

        public static IEnumerable<T> GetAttributes<T>(this MemberInfo member, bool includeInherited = true) where T : Attribute
        {
            return member.GetCustomAttributes(typeof(T), includeInherited).Select(a => a as T);
        }

        public static T GetAttribute<T>(this MemberInfo member, bool includeInherited = true) where T : Attribute
        {
            var result = member.GetAttributes<T>(includeInherited);
            return result.FirstOrDefault();
        }

        public static IEnumerable<T> GetAttributes<T>(this ParameterInfo param, bool includeInherited = true) where T : Attribute
        {
            var result = param.GetCustomAttributes(typeof(T), includeInherited);
            return result.Select(a => a as T);
        }

        public static T GetAttribute<T>(this ParameterInfo param, bool includeInherited = true) where T : Attribute
        {
            return param.GetAttributes<T>(includeInherited).FirstOrDefault();
        }

        public static bool HasAttribute<T>(this ParameterInfo param, bool includeInherited = true) where T : Attribute
        {
            return param.GetAttributes<T>(includeInherited).Any();
        }

        public static Type WithoutGenericArgs(this Type type)
        {
            return type.IsGenericType ? type.GetGenericTypeDefinition() : type;
        }

        /// <summary>
        /// Returns the T in List[T] or Task[T], for example, if contained in specified type. Otherwise returns self.
        /// </summary>
        public static Type UnwrapGeneric(this Type type, Type genericContainerType)
        {
            return (type?.WithoutGenericArgs() == genericContainerType)
                ? type?.GetGenericArguments().FirstOrDefault()
                : type;
        }
        
    }
}
