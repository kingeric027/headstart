using Marketplace.CMS.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.CMS.Storage
{
	public interface IStorage
	{
		Task<StorageAccount> Connect(string containerID);
		Task<Asset> UploadAsset(string containerID, IFormFile file, Asset asset);
		Task OnContainerDeleted(string containerID);
	}
}
