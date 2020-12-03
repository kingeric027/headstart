using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;
using ordercloud.integrations.library;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.WindowsAzure.Storage;

namespace ordercloud.integrations.cms
{
	public static class StorageHelper
	{
		public static async Task UploadFile(AssetContainer container, string blobName, IFormFile file)
		{
			await RunAction(container, async blobs =>
			{
				var block = blobs.GetBlockBlobReference(blobName);
				block.Properties.ContentType = file.ContentType;
				using (var stream = file.OpenReadStream())
				{
					await block.UploadFromStreamAsync(stream);
				}	
			});
		}

		public static async Task UploadImage(AssetContainer container, string blobName, Image image)
		{
			await RunAction(container, async blobs =>
			{
				var bytes = image.ToBytes(ImageFormat.Png);
				var block = blobs.GetBlockBlobReference(blobName);
				block.Properties.ContentType = "image/png";
				await block.UploadFromByteArrayAsync(bytes, 0, bytes.Length);
			});
		}

		public static async Task DeleteAsset(AssetContainer container, string blobName)
		{
			await RunAction(container, blobs => blobs.GetBlockBlobReference(blobName).DeleteIfExistsAsync());
		}

		private static async Task RunAction(AssetContainer assetContainer, Func<CloudBlobContainer, Task> action)
		{
			var blobContainer = await GetBlobContainer(assetContainer);
			try
			{
				await action(blobContainer);
			}
			catch (Exception ex)
			{
				throw new StorageConnectionException(assetContainer.id, ex);
			}
		}

		private static async Task<CloudBlobContainer> GetBlobContainer(AssetContainer assetContainer)
		{
			var containerName = $"assets-{assetContainer.id}";
			CloudStorageAccount.TryParse(assetContainer.Customer.StorageConnectionString, out var storage);
			var client = storage.CreateCloudBlobClient();
			var blobContainer = client.GetContainerReference(containerName);
			var isNewlyCreated = await blobContainer.CreateIfNotExistsAsync();
			if (isNewlyCreated)
			{
				var permissions = await blobContainer.GetPermissionsAsync();
				permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
				await blobContainer.SetPermissionsAsync(permissions);
			}
			return blobContainer;
		}
	}
}
