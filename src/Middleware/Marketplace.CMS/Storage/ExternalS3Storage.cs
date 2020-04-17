using Marketplace.CMS.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.CMS.Storage
{
	public class ExternalS3Storage : IStorage
	{
		public async Task<StorageAccount> Connect(string containerID)
		{
			throw new NotImplementedException();
		}

		public async Task<Asset> UploadAsset(string containerID, IFormFile file, Asset asset)
		{
			throw new NotImplementedException();
		}

		public async Task OnContainerDeleted(string containerID)
		{
			throw new NotImplementedException();
		}
	}
}
