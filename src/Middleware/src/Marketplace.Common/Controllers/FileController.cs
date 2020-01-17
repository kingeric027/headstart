using Marketplace.Common.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

		[HttpPost, Route("images/product/{productID}/{index}")]
		public async Task UploadProductImages(IFormFile file, string marketplaceID, string fileName, string index)
		{
			await _content.UploadProductImage(file, marketplaceID, fileName, index);
		}

		[HttpDelete, Route("images/product/{productID}/{index}")]
		public async Task DeleteProductImages(IFormFile file, string marketplaceID, string fileName, string index)
		{
			await _content.DeleteProductImage(marketplaceID, fileName, index);
		}
	}
}
