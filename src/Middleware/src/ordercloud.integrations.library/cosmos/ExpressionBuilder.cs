using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using ordercloud.integrations.library;

namespace ordercloud.integrations.library.Cosmos
{
    public static class ExpressionBuilder
    {
        private static Tuple<Expression, Type> GetExpression<T>(Expression param, string sort)
        {
            var member = param;
            member = sort.Replace("!", "").Split(".").Aggregate(member, Expression.Property); // takes X.X notation and gets to the nested property member

            var propertyType = ((PropertyInfo)member.To<MemberExpression>().Member).PropertyType;
            return Tuple.Create(member, propertyType);
        }

        private static Expression GetExpression<T>(Expression param, ListFilter filter)
        {
            var member = param;
            member = filter.Name.Split(".").Aggregate(member, Expression.Property); // takes X.X notation and gets to the nested property member

            var propertyType = ((PropertyInfo)member.To<MemberExpression>().Member).PropertyType;
            var converter = TypeDescriptor.GetConverter(propertyType);

            if (propertyType.IsEnum || (propertyType.IsNullable() && Nullable.GetUnderlyingType(propertyType).IsEnum))
                return GetEnumExpression<T>(propertyType, member, filter);
            return GetStringExpression<T>(converter, propertyType, member, filter);
        }

        private static BinaryExpression GetExpression<T>(Expression param, ListFilter filter1, ListFilter filter2)
        {
            var bin1 = GetExpression<T>(param, filter1);
            var bin2 = GetExpression<T>(param, filter2);

            return Expression.AndAlso(bin1, bin2);
        }

        public static Expression<Func<T, bool>> GetSearchExpression<T>(IListArgs args)
        {
            if (args?.Search == null || args?.SearchOn == null)
                return null;
            var param = Expression.Parameter(typeof(T), args?.SearchOn);

            var expr = GetExpression<T>(param, new ListFilter()
            {
                Name = args?.SearchOn,
                Values = new List<ListFilterValue>()
                {
                    new ListFilterValue() { Operator = ListFilterOperator.Equal, Term = args?.Search, WildcardPositions = new List<int>(){0,1} }
                }
            });
            return Expression.Lambda<Func<T, bool>>(expr, param);
        }

        public static Expression<Func<T, bool>> GetFilterExpression<T>(IListArgs args)
        {
            if (args?.Filters == null || args?.Filters.Count == 0)
                return null;
            var filters = args?.Filters;

            var param = Expression.Parameter(typeof(T), typeof(T).Name);

            Expression exp = null;

            switch (filters.Count)
            {
                case 1:
                    exp = GetExpression<T>(param, filters[0]);
                    break;
                case 2:
                    exp = GetExpression<T>(param, filters[0], filters[1]);
                    break;
                default:
                    {
                        while (filters.Count > 0)
                        {
                            var f1 = filters[0];
                            var f2 = filters[1];

                            exp = exp == null ?
                                GetExpression<T>(param, filters[0], filters[1]) :
                                Expression.AndAlso(exp, GetExpression<T>(param, filters[0], filters[1]));

                            filters.Remove(f1);
                            filters.Remove(f2);

                            if (filters.Count != 1) continue;

                            exp = Expression.AndAlso(exp, GetExpression<T>(param, filters[0]));
                            filters.RemoveAt(0);
                        }

                        break;
                    }
            }

            return Expression.Lambda<Func<T, bool>>(exp, param);
        }

        public static Tuple<Expression, Type> GetSortByExpression<TSource>(IListArgs args)
        {
            if (args?.SortBy.Count == 0)
                return null;
            var sort = args?.SortBy;
            var param = Expression.Parameter(typeof(TSource), typeof(TSource).Name);
            Tuple<Expression, Type> exp;
            switch (sort?.Count)
            {
                case 1:
                    exp = GetExpression<TSource>(param, sort[0]);
                    break;
                default:
                    throw new NotImplementedException("Multiple sort is not supported");

            }
            return Tuple.Create(Expression.Lambda(exp.Item1, param) as Expression, exp.Item2);
        }

        private static Expression GetEnumExpression<T>(Type propertyType, Expression member, ListFilter filter)
        {
            // TODO: this can't handle multiple values for single filter. ex: Action: Get|Ignore|Update
            var value = filter.Values.FirstOrDefault();
            if (value == null)
                return Expression.Empty();

            ConstantExpression right = null;
            if (propertyType.IsEnum)
                right = Expression.Constant((int)Enum.Parse(propertyType, value?.Term).To(propertyType));
            else if (Nullable.GetUnderlyingType(propertyType).IsEnum)
                right = Expression.Constant((int)Enum.Parse(propertyType.GenericTypeArguments[0], value?.Term).To(propertyType));

            //var right = Expression.Constant((int)Enum.Parse(propertyType, filter.Values.FirstOrDefault()?.Term).To(propertyType));
            var left = Expression.Convert(member, typeof(int));

            switch (value.Operator)
            {
                // doesn't yet support the | OR operator
                case ListFilterOperator.Equal:
                    return Expression.Equal(left, right);
                case ListFilterOperator.NotEqual:
                    return Expression.NotEqual(left, right);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static Expression GetStringExpression<T>(TypeConverter converter, Type propertyType, Expression member, ListFilter filter)
        {
            // TODO: this can't handle multiple values for single filter. ex: Action: Get|Ignore|Update
            var value = filter.Values.FirstOrDefault();
            if (value == null)
                return Expression.Empty();
            // works for strings and probably any non-complex object
            var propertyValue = converter.ConvertFromInvariantString(value?.Term);
            var constant = Expression.Constant(propertyValue?.ToString().To(propertyType));
            var right = Expression.Convert(constant, propertyType);

            // doesn't yet support the | OR operator
            switch (value.Operator)
            {
                case ListFilterOperator.Equal:
                    // * operator for start, contains and end
                    if (value.HasWildcard)
                    {
                        var term = "";
                        if (value.WildcardPositions.Count == 2)
                            term = "Contains";
                        else if (value.WildcardPositions[0] == 0)
                            term = "StartsWith";
                        else if (value.WildcardPositions[0] > 0)
                            term = "EndsWith";
                        var method = typeof(string).GetMethod(term, new[] { propertyType });
                        return Expression.Call(member, method, constant);
                    }
                    return Expression.Equal(member, right);
                case ListFilterOperator.NotEqual:
                    return Expression.NotEqual(member, right);
                case ListFilterOperator.GreaterThan:
                    return Expression.GreaterThan(member, right);
                case ListFilterOperator.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(member, right);
                case ListFilterOperator.LessThan:
                    return Expression.LessThan(member, right);
                case ListFilterOperator.LessThanOrEqual:
                    return Expression.LessThanOrEqual(member, right);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
