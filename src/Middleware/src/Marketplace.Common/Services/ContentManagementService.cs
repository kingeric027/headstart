using Microsoft.AspNetCore.Http;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Marketplace.Helpers.Services;
using Marketplace.Common.Helpers;
using System.Linq;
using Dynamitey;
using Marketplace.Common.Exceptions;
using Marketplace.Helpers.Models;

namespace Marketplace.Common.Services
{
	public interface IContentManagementService
	{
		Task<Product> UploadProductImage(IFormFile files, string marketplaceID, string productID, string token);
		Task<Product> DeleteProductImage(string marketplaceID, string productID, string fileName, string token);
	}

	public class ContentManagementService : IContentManagementService
	{
		private readonly BlobService _blob;
		private readonly AppSettings _settings;
		private readonly IOrderCloudClient _oc;

		public ContentManagementService(AppSettings settings) {
			_settings = settings;
			_oc = OcFactory.GetSEBAdmin();
			_blob = new BlobService(new BlobServiceConfig() {
				ConnectionString = settings.BlobSettings.ConnectionString,
				Container = "images"
			});
		}

		public async Task<Product> UploadProductImage(IFormFile file, string marketplaceID, string productID, string token)
		{
			var product = await _oc.Products.GetAsync<Product<ProductXp>>(productID, token);

			if (product?.xp?.Images == null)
				product.xp = new ProductXp { Images = new List<ProductImage>() };

			var index = product.xp.Images.Select(img => Int32.Parse(img.Url.Split('-').Last())).DefaultIfEmpty(0).Max() + 1;
			var blobName = GetProductImageName(marketplaceID, productID, index);
			_blob.Save(blobName, file);

			product.xp.Images.Add(new ProductImage()
			{
				Url = GetProductImageURL(blobName),
			});

			var partial = new PartialProduct()
			{
				xp = new
				{
					product.xp.Images,
				}
			};
			return await _oc.Products.PatchAsync(productID, partial, token);
		}

		public async Task<Product> DeleteProductImage(string marketplaceID, string productID, string fileName, string token)
		{
			var product = await _oc.Products.GetAsync<Product<ProductXp>>(productID, token);
			var blobName = GetProductImageName(marketplaceID, fileName);
			_blob.Delete(blobName);

			var Images = product.xp.Images.Where(img => !img.Url.EndsWith(fileName));

			return await _oc.Products.PatchAsync(productID, new PartialProduct() { xp = new { Images }}, token);
		}

		private string GetProductImageName(string mkplID, string imgName) => $"{mkplID}/products/{imgName}";
		private string GetProductImageName(string mkplID, string prodId, int i) => $"{mkplID}/products/{prodId}-{i}";
		private string GetProductImageURL(string blobName) => $"{_settings.BlobSettings.HostUrl}/images/{blobName}";
	}

	// TODO - move to nuget shared models
	public class ProductXp
	{
		public List<ProductImage> Images { get; set; }
	}

	public class ProductImage
	{
		public string Url { get; set; }
	}
}
