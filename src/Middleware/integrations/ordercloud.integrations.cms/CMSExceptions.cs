using System;
using System.Collections.Generic;
using System.Text;
using ordercloud.integrations.library;

namespace ordercloud.integrations.cms
{
	public class StorageConnectionException : OrderCloudIntegrationException
	{
		public StorageConnectionException(string containerInteropID, object ex) : base("Storage Connection", $"Container {containerInteropID} storage connection failed.", null) { }
	}

	public class AssetUploadValidationException : OrderCloudIntegrationException
	{
		public AssetUploadValidationException(string message) : base("Asset Upload Validation", message, null) { }
	}

	public class TokenExpiredException : OrderCloudIntegrationException
	{
		public TokenExpiredException() : base("Token", "Token has expired", null) { }
	}

	public class AllowedResourceAssociationsEmptyException : OrderCloudIntegrationException
	{
		public AllowedResourceAssociationsEmptyException(string schemaInteropID) : 
			base("Schema Error", $"AllowedResourceAssociations array cannot be empty", null) { }
	}

	public class DuplicateIDException : OrderCloudIntegrationException
	{
		public DuplicateIDException() : base("IdExists", "Object already exists.", null) { }
	}

}
