﻿using Integrations.CMS;
using Marketplace.CMS.Models;
using Marketplace.Helpers.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.CMS.Storage
{
	public interface IBlobStorage
	{
		CMSConfig Config { get; }
		Task<AssetContainer> OnContainerConnected(AssetContainer container);
		Task<Asset> UploadAsset(AssetContainer container, IFormFile file, Asset asset);
		Task OnContainerDeleted(AssetContainer container);
		Task OnAssetDeleted(AssetContainer container, string assetID);
	}

	public class BlobStorage : IBlobStorage
	{
		public CMSConfig Config { get; }

		public BlobStorage(CMSConfig config)
		{
			Config = config;
		}

		public async Task<AssetContainer> OnContainerConnected(AssetContainer container)
		{
			try
			{
				// https://docs.microsoft.com/en-us/azure/storage/blobs/storage-manage-access-to-resources
				await BuildBlobService(container).Init(BlobContainerPublicAccessType.Container);
				return container;
			} catch (Exception ex)
			{
				throw new StorageConnectionException(container.InteropID, ex);
			}
		}

		public async Task<Asset> UploadAsset(AssetContainer container, IFormFile file, Asset asset)
		{
			try
			{
				await BuildBlobService(container).Save(asset.id, file);
				return asset;
			}
			catch (Exception ex)
			{
				throw new StorageConnectionException(container.InteropID, ex);
			}
		}

		public async Task OnContainerDeleted(AssetContainer container)
		{
			try
			{
				await BuildBlobService(container).DeleteContainer();
			}
			catch (Exception ex)
			{
				throw new StorageConnectionException(container.InteropID, ex);
			}
		}

		public async Task OnAssetDeleted(AssetContainer container, string assetID)
		{
			try
			{
				await BuildBlobService(container).Delete(assetID);
			}
			catch (Exception ex)
			{
				throw new StorageConnectionException(container.InteropID, ex);
			}
		}

		private BlobService BuildBlobService(AssetContainer container)
		{
			return new BlobService(new BlobServiceConfig()
			{
				ConnectionString = Config.BlobStorageConnectionString,
				Container = $"assets-{container.id}"
			});
		}
	}
}
