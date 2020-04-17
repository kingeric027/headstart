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
		private readonly StorageAccount _account;

		public ExternalBlobStorage(StorageAccount account)
		{
			_account = account;
 		}

		public async Task<StorageAccount> Connect(string containerID)
		{
			// TODO - handle failure to connect
			await BuildBlobService(containerID).Init();
			return _account;
		}

		public async Task<Asset> UploadAsset(string containerID, IFormFile file, Asset asset)
		{
			// TODO - handle failure to connect
			await BuildBlobService(containerID).Save(asset.id, file);
			return asset;
		}

		public async Task OnContainerDeleted(string containerID)
		{
			// Do nothing - do not delete externally stored files.
		}

		private BlobService BuildBlobService(string containerID)
		{
			return new BlobService(new BlobServiceConfig()
			{
				ConnectionString = _account.ConnectionString,
				Container = $"assets-{containerID}"
			});
		}
	}
}
