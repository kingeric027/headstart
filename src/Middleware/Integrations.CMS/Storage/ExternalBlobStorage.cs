using Marketplace.CMS.Models;
using Marketplace.Helpers.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.CMS.Storage
{
	public class ExternalBlobStorage : IStorage
	{
		private readonly AssetContainer _container;

		public ExternalBlobStorage(AssetContainer container)
		{
			_container = container;
		}

		public async Task<AssetContainer> OnContainerConnected()
		{
			throw new NotImplementedException();
		}

		public async Task<Asset> UploadAsset(IFormFile file, Asset asset)
		{
			throw new NotImplementedException();
		}

		public async Task OnContainerDeleted()
		{
			throw new NotImplementedException();
		}

		public async Task OnAssetDeleted(string assetID)
		{
			throw new NotImplementedException();
		}
	}
}
