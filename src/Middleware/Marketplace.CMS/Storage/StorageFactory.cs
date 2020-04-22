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
		Task<AssetContainer> OnContainerConnected();
		Task<Asset> UploadAsset(IFormFile file, Asset asset);
		Task OnContainerDeleted();
	}

	public interface IStorageFactory
	{
		IStorage GetStorage(AssetContainer container);
	}

	public class StorageFactory: IStorageFactory
	{
		private readonly AppSettings _settings;

		public StorageFactory(AppSettings settings)
		{
			_settings = settings;
		}

		public IStorage GetStorage(AssetContainer container)
		{
			switch (container?.StorageAccount?.Type)
			{
				//case StorageAccountType.ExternalBlob:
				//	return new ExternalBlobStorage(container.StorageAccount);
				//case StorageAccountType.ExternalS3:
				//	return new ExternalS3Storage(container.StorageAccount);
				case StorageAccountType.DefaultBlob:
				default:
					return new DefaultBlobStorage(container, _settings);
			}
		}
	}
}
