using Marketplace.CMS.Models;
using Marketplace.CMS.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Common.Controllers.CMS
{
	[Route("catalogs")]
	public class CatalogControllers : AbstractContentController
	{
		protected override Resource ContentConfig { get; set; } = new Resource(ResourceType.Catalogs);

		public CatalogControllers(AppSettings settings, IAssetedResourceQuery assetedResources) : base(settings, assetedResources){}
	}

	[Route("catelogs/{catelogID}/categories")]
	public class CategoryController : AbstractContentController
	{
		protected override Resource ContentConfig { get; set; } = new Resource(ResourceType.Categories);

		public CategoryController(AppSettings settings, IAssetedResourceQuery assetedResources) : base(settings, assetedResources) { }
	}

	[Route("promotions")]
	public class PromotionControllers : AbstractContentController
	{
		protected override Resource ContentConfig { get; set; } = new Resource(ResourceType.Promotions);

		public PromotionControllers(AppSettings settings, IAssetedResourceQuery assetedResources) : base(settings, assetedResources) { }
	}

	[Route("buyers")]
	public class BuyerController : AbstractContentController
	{
		protected override Resource ContentConfig { get; set; } = new Resource(ResourceType.Buyers);

		public BuyerController(AppSettings settings, IAssetedResourceQuery assetedResources) : base(settings, assetedResources) { }
	}

	[Route("productfacets")]
	public class FacetController : AbstractContentController
	{
		protected override Resource ContentConfig { get; set; } = new Resource(ResourceType.ProductFacets);

		public FacetController(AppSettings settings, IAssetedResourceQuery assetedResources) : base(settings, assetedResources) { }
	}


}
