using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;
using ordercloud.integrations.library;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace ordercloud.integrations.cms
{
	public static class StorageHelper
	{
		// Keys are container ids. 
		private static ConcurrentDictionary<string, OrderCloudIntegrationsBlobService> storageConnections = 
			new ConcurrentDictionary<string, OrderCloudIntegrationsBlobService>();
		
		public static async Task UploadFile(AssetContainer container, string blobName, IFormFile file)
		{
			await RunAction(container, blobs => blobs.Save(blobName, file));
		}

		public static async Task UploadImage(AssetContainer container, string blobName, Image image)
		{
			await RunAction(container, async blobs => {
				var bytes = image.ToBytes(ImageFormat.Png);
				await blobs.Save(blobName, bytes, "image/png");
			});
		}

		public static async Task DeleteAsset(AssetContainer container, string blobName)
		{
			await RunAction(container, blobs => blobs.Delete(blobName));
		}

		private static async Task RunAction(AssetContainer container, Func<OrderCloudIntegrationsBlobService, Task> action)
		{
			if (!storageConnections.ContainsKey(container.id))
			{
				var blobService = new OrderCloudIntegrationsBlobService(new BlobServiceConfig()
				{
					ConnectionString = container.Customer.StorageConnectionString,
					Container = $"assets-{container.id}", // SellerOrgID can contain "_", an illegal character for blob containers.
					AccessType = BlobContainerPublicAccessType.Container
				});
				storageConnections.TryAdd(container.id, blobService);
			}
			try
			{
				await action(storageConnections[container.id]);
			}
			catch (Exception ex)
			{
				throw new StorageConnectionException(container.id, ex);
			}
		}
	}
}
