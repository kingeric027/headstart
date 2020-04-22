using Marketplace.CMS.Models;
using Marketplace.Common;
using Marketplace.Helpers.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.CMS.Storage
{
	public class DefaultBlobStorage : IStorage
	{
		private readonly AssetContainer _container;

		public DefaultBlobStorage(AssetContainer container, AppSettings settings)
		{
			_container = container;
			_container.StorageAccount = new StorageAccount()
			{
				Type = StorageAccountType.DefaultBlob,
				HostUrl = settings.BlobSettings.HostUrl,
				ConnectionString = settings.BlobSettings.ConnectionString,
			};
		}

		public async Task<AssetContainer> OnContainerConnected()
		{
			try
			{
				// https://docs.microsoft.com/en-us/azure/storage/blobs/storage-manage-access-to-resources
				await BuildBlobService(_container.id).Init(BlobContainerPublicAccessType.Container);
				return _container;
			} catch (Exception ex)
			{
				throw new StorageConnectionException(_container.InteropID, ex);
			}
		}

		public async Task<Asset> UploadAsset(IFormFile file, Asset asset)
		{
			try
			{
				await BuildBlobService(_container.id).Save(asset.id, file);
				return asset;
			}
			catch (Exception ex)
			{
				throw new StorageConnectionException(_container.InteropID, ex);
			}
		}

		public async Task OnContainerDeleted()
		{
			try
			{
				await BuildBlobService(_container.id).DeleteContainer();
			}
			catch (Exception ex)
			{
				throw new StorageConnectionException(_container.InteropID, ex);
			}
		}
	
		private BlobService BuildBlobService(string containerID)
		{
			return new BlobService(new BlobServiceConfig()
			{
				ConnectionString = _container.StorageAccount.ConnectionString,
				Container = $"assets-{containerID}"
			});
		}
	}
}
