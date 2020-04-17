using Cosmonaut;
using Cosmonaut.Extensions;
using Marketplace.CMS.Models;
using Marketplace.Helpers;
using Marketplace.Helpers.Extensions;
using Marketplace.Helpers.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Marketplace.Helpers.Exceptions.ApiErrorException;

namespace Marketplace.CMS.Queries
{
	public interface IAssetContainerQuery
	{
		Task<ListPage<AssetContainer>> List(IListArgs args);
		Task<AssetContainer> Get(string interopID);
		Task<AssetContainer> Create(AssetContainer container);
		Task<AssetContainer> CreateOrUpdate(string interopID, AssetContainer container);
		Task Delete(string interopID);
	}

	public class AssetContainerQuery: IAssetContainerQuery
	{
		private readonly ICosmosStore<AssetContainer> _store;
		public const string SinglePartitionID = "SinglePartitionID";

		public AssetContainerQuery(ICosmosStore<AssetContainer> store)
		{
			_store = store;
		}

		public async Task<ListPage<AssetContainer>> List(IListArgs args)
		{
			var query = _store.Query(new FeedOptions() { EnableCrossPartitionQuery = false })
				.Search(args)
				.Filter(args)
				.Sort(args);
			var list = await query.WithPagination(args.Page, args.PageSize).ToPagedListAsync();
			var count = await query.CountAsync();
			return list.ToListPage(args.Page, args.PageSize, count);
		}

		public async Task<AssetContainer> Get(string interopID)
		{
			var item = await GetWithoutExceptions(interopID);
			return item ?? throw new NotFoundException("AssetContainer", interopID);
		}

		public async Task<AssetContainer> Create(AssetContainer container)
		{
			var item = await GetWithoutExceptions(container.InteropID);
			if (item != null) throw new DuplicateIdException();
			return await Save(container);
		}

		public async Task<AssetContainer> CreateOrUpdate(string interopID, AssetContainer container)
		{
			var item = await GetWithoutExceptions(container.InteropID);
			container.id = 
		}

		public async Task Delete(string interopID)
		{
			var item = await Get(interopID);
			await _store.RemoveByIdAsync(item.id, SinglePartitionID);
		}

		private async Task<AssetContainer> Save(AssetContainer container)
		{
			container.timeStamp = DateTime.Now;
			var result = await _store.UpsertAsync(container);
			return result.Entity;
		}

		private async Task<AssetContainer> GetWithoutExceptions(string interopID)
		{
			var item = await _store.Query($"select top 1 * from c where c.InteropID = @id", new { id = interopID }).FirstOrDefaultAsync();
			return item;
		}
	}
}
