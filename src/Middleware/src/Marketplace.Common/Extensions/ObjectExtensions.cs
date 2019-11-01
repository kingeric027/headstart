using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Marketplace.Common.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// The most useful method known to mankind
        /// </summary>
        public static T To<T>(this object obj)
        {
            return (T)obj.To(typeof(T));
        }

        public static object To(this object obj, Type type)
        {
            if (obj == null || obj == DBNull.Value)
                return null;

            if (type.IsInstanceOfType(obj))
                return obj;

            if (type.IsNullable())
                type = type.GetGenericArguments()[0];

            if (type.IsEnum)
                return Enum.Parse(type, obj.ToString(), true);

            if (obj is JToken jt)
                return jt.ToObject(type);

            if (type.UnderlyingSystemType == typeof(DateTimeOffset)) // TODO: figure out how to evaluate the actual Type
                return DateTimeOffset.Parse(obj.ToString());

            return Convert.ChangeType(obj, type);
        }

        public static bool IsNullable(this Type type)
        {
            return type != null && type.WithoutGenericArgs() == typeof(Nullable<>);
        }
    }
}
