using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Helpers.Services;
using Marketplace.Common.Helpers;
using System.Linq;
using Marketplace.Models;
using Marketplace.Models.Extended;
using Marketplace.Helpers.Models;
using Marketplace.Helpers;
using Marketplace.Common.Queries;
using Marketplace.Common.Models;

namespace Marketplace.Common.Services
{
	public interface IContentManagementService
	{
		Task<SuperMarketplaceProduct> UploadProductImage(IFormFile files, string marketplaceID, string productID, string token);
		Task<SuperMarketplaceProduct> DeleteProductImage(string marketplaceID, string productID, string fileName, string token);
		Task<SuperMarketplaceProduct> UploadStaticContent(IFormFile file, string productID, string fileName, string token);
		Task<SuperMarketplaceProduct> DeleteStaticContent(string marketplaceID, string productID, string fileName, string token);
	}

	public class ContentManagementService : IContentManagementService
	{
		private readonly AppSettings _settings;
		private readonly IOrderCloudClient _oc;
		private readonly IBlobService _imageContainer;
		private readonly IBlobService _staticContentContainer;
		private readonly IImageQuery _img;
		private readonly IImageProductAssignmentQuery _ipa;

		public ContentManagementService(AppSettings settings, IOrderCloudClient oc, IImageQuery img, IImageProductAssignmentQuery ipa)
		{
			_settings = settings;
			_oc = oc;
			// TODO: move this down to where we're injecting this service
			_imageContainer = new BlobService(new BlobServiceConfig()
			{
				ConnectionString = settings.BlobSettings.ConnectionString,
				Container = "images"
			});
			_staticContentContainer = new BlobService(new BlobServiceConfig()
			{
				ConnectionString = settings.BlobSettings.ConnectionString,
				Container = "static-content"
			});
			_img = img;
			_ipa = ipa;
		}

		public async Task<SuperMarketplaceProduct> UploadProductImage(IFormFile file, string marketplaceID, string productID, string token)
		{
			var product = await _oc.Products.GetAsync<MarketplaceProduct>(productID, token);
			if (product.xp?.Images == null)
				product.xp = new ProductXp { Images = new List<ProductImage>() };

			var index = product.xp.Images.Select(img => int.Parse(img.URL.Split('-').Last())).DefaultIfEmpty(0).Max() + 1;
			var blobName = GetProductImageName(marketplaceID, productID, index);
			await _imageContainer.Save(blobName, file);

			product.xp?.Images?.Add(new ProductImage()
			{
				URL = GetProductImageURL(blobName),
			});

			// Add Image and ImageProductAssignment records to Cosmos
			var newImage = await _img.Save(new Image { id = $"{productID}-{index}", URL = GetProductImageURL(blobName), ListOrder = index });
			await _ipa.Save(new ImageProductAssignment { id = $"{newImage.id}-{productID}", ImageID = newImage.id, ProductID = productID });

			var partial = new PartialProduct()
			{
				xp = new
				{
					product.xp.Images
				}
			};
			var _patchedProduct = await _oc.Products.PatchAsync<MarketplaceProduct>(productID, partial, token);
			var _priceSchedule = await _oc.PriceSchedules.GetAsync<PriceSchedule>(product.DefaultPriceScheduleID);
			return new SuperMarketplaceProduct
			{
				Product = _patchedProduct,
				PriceSchedule = _priceSchedule
			};
		}

		public async Task<SuperMarketplaceProduct> DeleteProductImage(string marketplaceID, string productID, string fileName, string token)
		{
			var product = await _oc.Products.GetAsync<MarketplaceProduct>(productID, token);
			var blobName = GetProductImageName(marketplaceID, fileName);
			await _imageContainer.Delete(blobName);

			var Images = product.xp.Images.Where(img => !img.URL.EndsWith(fileName));

			var _patchedProduct = await _oc.Products.PatchAsync<MarketplaceProduct>(productID, new PartialProduct() { xp = new { Images } }, token);
			var _priceSchedule = await _oc.PriceSchedules.GetAsync<PriceSchedule>(product.DefaultPriceScheduleID);
			return new SuperMarketplaceProduct
			{
				Product = _patchedProduct,
				PriceSchedule = _priceSchedule
			};
		}

		public async Task<SuperMarketplaceProduct> UploadStaticContent(IFormFile file, string productID, string fileName, string token)
		{
			/* TODO: Fix Problem with parallel requests */
			var product = await _oc.Products.GetAsync<MarketplaceProduct>(productID, token);
			if (product.xp?.StaticContent == null)
				product.xp = new ProductXp { StaticContent = new List<StaticContent>() };
			Require.That(product.xp.StaticContent.Count < 10, new ErrorCode("Exceeds document limit on product", 400, "A product can have up to 10 associated documents."));
			await _staticContentContainer.Save(fileName, file);

			product.xp?.StaticContent?.Add(new StaticContent()
			{
				Title = fileName,
				URL = GetStaticContentURL(productID, fileName),
			});

			var partial = new PartialProduct()
			{
				xp = new
				{
					product.xp.StaticContent
				}
			};
			var _patchedProduct = await _oc.Products.PatchAsync<MarketplaceProduct>(productID, partial, token);
			var _priceSchedule = await _oc.PriceSchedules.GetAsync<PriceSchedule>(product.DefaultPriceScheduleID);
			return new SuperMarketplaceProduct
			{
				Product = _patchedProduct,
				PriceSchedule = _priceSchedule
			};
		}

		public async Task<SuperMarketplaceProduct> DeleteStaticContent(string marketplaceID, string productID, string fileName, string token)
		{
			var product = await _oc.Products.GetAsync<MarketplaceProduct>(productID, token);
			var blobName = GetProductStaticContentName(marketplaceID, productID, fileName);
			await _staticContentContainer.Delete(blobName);

			var StaticContent = product.xp.StaticContent.Where(file => !file.URL.EndsWith(fileName));

			var _patchedProduct = await _oc.Products.PatchAsync<MarketplaceProduct>(productID, new PartialProduct() { xp = new { StaticContent } }, token);
			var _priceSchedule = await _oc.PriceSchedules.GetAsync<PriceSchedule>(product.DefaultPriceScheduleID);
			return new SuperMarketplaceProduct
			{
				Product = _patchedProduct,
				PriceSchedule = _priceSchedule
			};
		}

		private string GetProductImageName(string mkplID, string imgName) => $"{mkplID}/products/{imgName}";
		private string GetProductImageName(string mkplID, string prodId, int? i) => $"{mkplID}/products/{prodId}-{i}";
		private string GetProductImageURL(string blobName) => $"{_settings.BlobSettings.HostUrl}/images/{blobName}";
		private string GetProductStaticContentName(string mkplID, string productID, string fileName) => $"{mkplID}/static-content/{productID}/{fileName}";
		private string GetStaticContentURL(string productID, string fileName) => $"{_settings.BlobSettings.HostUrl}/static-content/{productID}/{fileName}";
	}
}