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
		private readonly StorageAccount _account;

		public ExternalS3Storage(StorageAccount account)
		{
			_account = account;
		}

		public async Task<StorageAccount> OnContainerConnected(string containerID)
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
