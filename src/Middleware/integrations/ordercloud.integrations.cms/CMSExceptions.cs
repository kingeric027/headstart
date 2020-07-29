using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ordercloud.integrations.library;

namespace ordercloud.integrations.cms
{
	public class StorageConnectionException : OrderCloudIntegrationException
	{
		public StorageConnectionException(string containerInteropID, object ex) : base("Storage Connection", $"Container \"{containerInteropID}\" storage connection failed.", null) { }
	}

	public class AssetUploadValidationException : OrderCloudIntegrationException
	{
		public AssetUploadValidationException(string message) : base("Asset Upload Validation", message, null) { }
	}

	public class TokenExpiredException : OrderCloudIntegrationException
	{
		public TokenExpiredException() : base("Token", "Token has expired", null) { }
	}

	public class DuplicateIDException : OrderCloudIntegrationException
	{
		public DuplicateIDException() : base("IdExists", "Object already exists.", null) { }
	}

	public class SchemaNotValidException : OrderCloudIntegrationException
	{
		public SchemaNotValidException(IList<string> errors) : base("Schema Invalid", "Errors with Json Schema", errors) { }
	}

	public class DocumentNotValidException : OrderCloudIntegrationException
	{
		public DocumentNotValidException(string schemaInteropID, IList<string> errors) : base("Document Invalid", $"This Document does not conform to schema \"{schemaInteropID}\"", errors) { }
	}

	public class InvalidAssignmentException : OrderCloudIntegrationException
	{
		public InvalidAssignmentException(List<ResourceType> allowed) : 
			base("Invalid Assignment", $"This type of document can only be assigned the following resources. This is set on the schema.", allowed.Select(r => Enum.GetName(r.GetType(), r))) { }
	}
}
