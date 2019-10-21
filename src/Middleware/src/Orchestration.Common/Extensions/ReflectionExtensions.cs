using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Orchestration.Common.Extensions
{
    public static class ReflectionExtensions
    {
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
    }
}
