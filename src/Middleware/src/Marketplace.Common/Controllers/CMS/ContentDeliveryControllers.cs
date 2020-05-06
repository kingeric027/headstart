using Marketplace.CMS.Models;
using Marketplace.CMS.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Common.Controllers.CMS
{
	[Route("catalogs")]
	public class CatalogControllers : AbstractContentDeliveryController
	{
		protected override ContentControllerConfig config { get; set; } = new ContentControllerConfig()
		{
			ResourceType = ResourceType.Catalogs
		};
		public CatalogControllers(AppSettings settings, IAssetedResourceQuery assetedResources) : base(settings, assetedResources){}
	}
}
