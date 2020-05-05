﻿using Marketplace.CMS.Models;
using Marketplace.CMS.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Common.Controllers.CMS
{
	[Route("catalog/{catalogID}")]
	public class CatalogControllers : AbstractContentController
	{
		protected override ContentControllerConfig config { get; set; } = new ContentControllerConfig()
		{
			ResourceType = ResourceType.Catalogs,
			ResourceIDField = "catalogID",
			ResourceParentIDField = null
		};
		public CatalogControllers(AppSettings settings, IAssetedResourceQuery assetedResources) : base(settings, assetedResources)
		{
	
		}
	}
}
