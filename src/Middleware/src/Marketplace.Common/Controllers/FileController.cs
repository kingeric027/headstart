using Marketplace.Common.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Helpers.Attributes;
using Marketplace.Models;
using Marketplace.Models.Attributes;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Files\" represents files for Marketplace content management control")]
    [MarketplaceSection.Marketplace(ListOrder = 4)]
    [Route("{marketplaceID}")]
	public class FileController: BaseController
	{
		private readonly IContentManagementService _content;

		public FileController(IContentManagementService content, AppSettings settings) : base(settings) {
			_content = content;
		}

        [DocName("POST Product Images")]
		[HttpPost, Route("images/product/{productID}"), MarketplaceUserAuth(ApiRole.ProductAdmin)]
		public async Task<SuperMarketplaceProduct> Post([FromForm] IFormFile file, string marketplaceID, string productID)
		{
			return await _content.UploadProductImage(file, marketplaceID, productID, VerifiedUserContext.AccessToken);
		}

        [DocName("DELETE Product Images")]
		[HttpDelete, Route("images/product/{productID}/{fileName}"), MarketplaceUserAuth(ApiRole.ProductAdmin)]
		public async Task<SuperMarketplaceProduct> Delete(string marketplaceID, string productID, string fileName)
		{
			return await _content.DeleteProductImage(marketplaceID, productID, fileName, VerifiedUserContext.AccessToken);
		}
	}
}
