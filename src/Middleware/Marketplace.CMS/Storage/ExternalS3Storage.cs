using Marketplace.CMS.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.CMS.Storage
{
	public class ExternalS3Storage : IStorage
	{
		private readonly AssetContainer _container;

		public ExternalS3Storage(AssetContainer container)
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
