using Microsoft.AspNetCore.Http;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Marketplace.Helpers.Services;

namespace Marketplace.Common.Services
{
	public interface IContentManagementService
	{
		Task UploadProductImage(IFormFile files, string marketplaceID, string productID, string index);
		Task DeleteProductImage(string marketplaceID, string productID, string index);
	}

	public class ContentManagementService : IContentManagementService
	{
		private readonly BlobService _blob;

		public ContentManagementService(AppSettings settings) {
			_blob = new BlobService(new BlobServiceConfig() { 
				ConnectionString = settings.BlobSettings.ConnectionString,
				Container = "images" 
			});
		}

		public async Task UploadProductImage(IFormFile file, string marketplaceID, string productID, string index)
		{
			var blobName = GetProductImageName(marketplaceID, productID, index);
			await _blob.Save(blobName, file);
		}

		public async Task DeleteProductImage(string marketplaceID, string productID, string index)
		{
			var blobName = GetProductImageName(marketplaceID, productID, index);
			await _blob.Delete(blobName);
		}

		private string GetProductImageName(string mkplID, string prodID, string i) => $"{mkplID}/products/{prodID}-{i}";
	}
}
