using Cosmonaut;
using Cosmonaut.Extensions;
using Microsoft.Azure.Documents.Client;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ordercloud.integrations.cms
{
	public static class CosmosExtensions
	{
		// Like the native store.Find() except it takes in multiple ids and return multiple documents 
		public static async Task<ListPage<T>> FindMultipleAsync<T>(this ICosmosStore<T> store, IEnumerable<string> ids, ListArgsPageOnly args, FeedOptions options = null) where T : class
		{
			var uniqueIds = new HashSet<string>(ids).ToList(); // remove any duplicates
			var count = uniqueIds.Count;
			if (count == 0) return new ListPage<T>().Empty();
			var paramNames = uniqueIds.Select((id, i) => $"@id{i}");
			var parameters = new ExpandoObject();
			for (int i = 0; i < count; i++)
			{
				parameters.TryAdd($"@id{i}", uniqueIds[i]);
			}
			var query = store.Query($"select * from c where c.id IN ({string.Join(", ", paramNames)})", parameters, options);
			var list = await query.WithPagination(args.Page, args.PageSize).ToPagedListAsync();
			return list.ToListPage(args.Page, args.PageSize, list.Results.Count);
		}
	}
}
