using Marketplace.CMS.Models;
using Marketplace.Common;
using Marketplace.Helpers.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.CMS.Storage
{
	public class DefaultBlobStorage : IStorage
	{
		private readonly StorageAccount _account;

		public DefaultBlobStorage(AppSettings settings)
		{
			_account = new StorageAccount()
			{
				Type = StorageAccountType.DefaultBlob,
				HostUrl = settings.BlobSettings.HostUrl,
				ConnectionString = settings.BlobSettings.ConnectionString,
			};
		}

		public async Task<StorageAccount> OnContainerConnected(string containerID)
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
			await BuildBlobService(containerID).DeleteContainer();
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
