using Cosmonaut;
using Microsoft.Azure.Documents.Client;
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
		public static async Task<IEnumerable<T>> FindMultipleAsync<T>(this ICosmosStore<T> store, IEnumerable<string> ids, FeedOptions options = null) where T : class
		{
			var uniqueIds = new HashSet<string>(ids).ToList(); // remove any duplicates
			if (uniqueIds.Count == 0) return new List<T>();
			var paramNames = uniqueIds.Select((id, i) => $"@id{i}");
			var parameters = new ExpandoObject();
			for (int i = 0; i < uniqueIds.Count; i++)
			{
				parameters.TryAdd($"@id{i}", uniqueIds[i]);
			}
			var documents = await store.QueryMultipleAsync($"select * from c where c.id IN ({string.Join(", ", paramNames)})", parameters, options);
			return documents;
		}
	}
}
