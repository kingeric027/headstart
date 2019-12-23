using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Extensions
{
	public static class LINQExtensions
	{
		public static async Task<IEnumerable<TResult>> SelectAsync<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Task<TResult>> method)
		{
			return await Task.WhenAll(source.Select(async s => await method(s)));
		}
	}
}
