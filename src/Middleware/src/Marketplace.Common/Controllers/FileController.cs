using Marketplace.Common.Services;
using Marketplace.Helpers;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Controllers
{
	[Route("{marketplaceID}")]
	public class FileController: BaseController
	{
		private readonly IContentManagementService _content;

		public FileController(IContentManagementService content, AppSettings settings) : base(settings) {
			_content = content;
		}

		[HttpPost, Route("images/product/{productID}"), MarketplaceUserAuth(ApiRole.ProductAdmin)]
		public async Task<Product> UploadProductImages(IFormFile file, string marketplaceID, string productID)
		{
			return await _content.UploadProductImage(file, marketplaceID, productID, VerifiedUserContext.AccessToken);
		}

		[HttpDelete, Route("images/product/{productID}/{fileName}"), MarketplaceUserAuth(ApiRole.ProductAdmin)]
		public async Task<Product> DeleteProductImages(string marketplaceID, string productID, string fileName)
		{
			return await _content.DeleteProductImage(marketplaceID, productID, fileName, VerifiedUserContext.AccessToken);
		}
	}
}
