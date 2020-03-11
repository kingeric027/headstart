using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Marketplace.Helpers.Models;
using Newtonsoft.Json.Linq;

namespace Marketplace.Helpers.Extensions
{
    public static class ObjectExtensions
    {
 
        public static bool IsCollection(this Type type)
        {
            return GetCollectionItemType(type) != null;
        }

        /// <summary>
        /// Gets the item type of a collection type, i.e. the T in List&lt;T&gt; or T[], or null if it's not a collection.
        /// </summary>
        public static Type GetCollectionItemType(this Type type)
        {
            if (type.IsArray)
                return type.GetElementType();
            if (type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type))
                return type.GetGenericArguments()[0];
            return null;
        }
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

        /// <summary>
        /// if it's a nullable type, return underlying type. otherwise return self.
        /// </summary>
        public static Type WithoutNullable(this Type type)
        {
            return IsNullable(type) ? Nullable.GetUnderlyingType(type) : type;
        }

        public static bool IsNullable(this Type type)
        {
            return type != null && type.WithoutGenericArgs() == typeof(Nullable<>);
        }

        public static bool IsNumeric(this Type type)
        {
            if (type == null || type.IsEnum)
                return false;

            switch (Type.GetTypeCode(type.WithoutNullable()))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }
    }
}
