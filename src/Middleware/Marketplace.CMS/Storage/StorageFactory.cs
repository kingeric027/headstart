using Marketplace.CMS.Models;
using Marketplace.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.CMS.Storage
{
	public interface IStorage
	{
		Task<StorageAccount> OnContainerConnected(string containerID);
		Task<Asset> UploadAsset(string containerID, IFormFile file, Asset asset);
		Task OnContainerDeleted(string containerID);
	}

	public interface IStorageFactory
	{
		IStorage GetStorage(StorageAccount account);
	}

	public class StorageFactory: IStorageFactory
	{
		private readonly AppSettings _settings;

		public StorageFactory(AppSettings settings)
		{
			_settings = settings;
		}

		public IStorage GetStorage(StorageAccount account)
		{
			switch (account?.Type)
			{
				case StorageAccountType.DefaultBlob:
					return new DefaultBlobStorage(_settings);
				//case StorageAccountType.ExternalBlob:
				//	return new ExternalBlobStorage(container.StorageAccount);
				//case StorageAccountType.ExternalS3:
				//	return new ExternalS3Storage(container.StorageAccount);
				default:
					return new DefaultBlobStorage(_settings);
			}
		}
	}
}
