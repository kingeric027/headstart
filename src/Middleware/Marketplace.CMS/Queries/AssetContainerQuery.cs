using Cosmonaut;
using Cosmonaut.Extensions;
using Marketplace.CMS.Models;
using Marketplace.Helpers;
using Marketplace.Helpers.Extensions;
using Marketplace.Helpers.Models;
using Microsoft.Azure.Documents.Client;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.CMS.Queries
{
	public interface IAssetContainerQuery
	{
		Task<ListPage<AssetContainer>> List(IListArgs args);
		Task<AssetContainer> Get(string containerId);
		Task<AssetContainer> Save(AssetContainer container);
		Task Delete(string id);
	}

	public class AssetContainerQuery: IAssetContainerQuery
	{
		private readonly ICosmosStore<AssetContainer> _store;

		public AssetContainerQuery(ICosmosStore<AssetContainer> store)
		{
			_store = store;
		}

		public async Task<ListPage<AssetContainer>> List(IListArgs args)
		{
			var query = _store.Query(new FeedOptions() { EnableCrossPartitionQuery = true })
				.Search(args)
				.Filter(args)
				.Sort(args);
			var list = await query.WithPagination(args.Page, args.PageSize).ToPagedListAsync();
			var count = await query.CountAsync();
			return list.ToListPage(args.Page, args.PageSize, count);
		}

		public async Task<AssetContainer> Get(string id)
		{
			var item = await _store.FindAsync(id);
			return item;
		}

		public async Task<AssetContainer> Save(AssetContainer log)
		{
			log.timeStamp = DateTime.Now;
			var result = await _store.UpsertAsync(log);
			return result.Entity;
		}

		public async Task Delete(string id)
		{
			await _store.RemoveByIdAsync(id);
		}
	}
}
