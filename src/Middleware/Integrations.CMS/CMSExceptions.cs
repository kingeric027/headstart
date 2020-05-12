using Marketplace.CMS.Models;
using System;
using System.Collections.Generic;
using System.Text;
using ordercloud.integrations.extensions;

namespace Marketplace.CMS
{
	public class StorageConnectionException : OrderCloudIntegrationException
	{
		public StorageConnectionException(string containerInteropID, object ex) : base("Storage Connection", 400, $"Container {containerInteropID} storage connection failed.", ex) { }
	}

	public class AssetUploadValidationException : OrderCloudIntegrationException
	{
		public AssetUploadValidationException(string message) : base("Asset Upload Validation", 400, message, null) { }
	}

	public class TokenExpiredException : OrderCloudIntegrationException
	{
		public TokenExpiredException() : base("Token", 400, "Token has expired", null) { }
	}
}
