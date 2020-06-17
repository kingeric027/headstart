using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.Models;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands.Crud
{
	public interface IMarketplaceCatalogCommand
	{
		Task<MarketplaceCatalog> Get(string buyerID, string catalogID, VerifiedUserContext user);
		Task<ListPage<MarketplaceCatalog>> List(string buyerID, ListArgs<MarketplaceCatalog> args, VerifiedUserContext user);
		Task<MarketplaceCatalog> Post(string buyerID, MarketplaceCatalog catalog, VerifiedUserContext user);
		Task<MarketplaceCatalog> Put(string buyerID, string catalogID, MarketplaceCatalog catalog, VerifiedUserContext user);
		Task Delete(string buyerID, string catalogID, VerifiedUserContext user);
	}

	public class MarketplaceCatalogCommand : IMarketplaceCatalogCommand
	{
		private readonly IOrderCloudClient _oc;
		public MarketplaceCatalogCommand(AppSettings settings, IOrderCloudClient oc)
		{
			_oc = oc;
		}

		public async Task<MarketplaceCatalog> Get(string buyerID, string catalogID, VerifiedUserContext user)
		{
			return await _oc.UserGroups.GetAsync<MarketplaceCatalog>(buyerID, catalogID, user.AccessToken);
		}

		public async Task<ListPage<MarketplaceCatalog>> List(string buyerID, ListArgs<MarketplaceCatalog> args, VerifiedUserContext user)
		{
			var queryParamsForCatalogUserGroup = new Tuple<string, string>("xp.Type", "Catalog");
			args.Filters.Add(new ListFilter()
			{
				QueryParams = new List<Tuple<string, string>> { queryParamsForCatalogUserGroup }
			});
			return await _oc.UserGroups.ListAsync<MarketplaceCatalog>(buyerID, filters: args.ToFilterString(),
				search: args.Search,
				pageSize: args.PageSize,
				page: args.Page,
				accessToken: user.AccessToken);
		}

		public async Task<MarketplaceCatalog> Post(string buyerID, MarketplaceCatalog catalog, VerifiedUserContext user)
		{
			return await _oc.UserGroups.CreateAsync<MarketplaceCatalog>(buyerID, catalog, user.AccessToken);
		}

		public async Task<MarketplaceCatalog> Put(string buyerID, string catalogID, MarketplaceCatalog catalog, VerifiedUserContext user)
		{
			return await _oc.UserGroups.SaveAsync<MarketplaceCatalog>(buyerID, catalogID, catalog, user.AccessToken);
		}

		public async Task Delete(string buyerID, string catalogID, VerifiedUserContext user)
		{
			await _oc.UserGroups.DeleteAsync(buyerID, catalogID, user.AccessToken);
		}
	}
}
