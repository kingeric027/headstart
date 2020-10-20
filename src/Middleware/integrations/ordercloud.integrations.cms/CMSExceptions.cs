﻿using System;
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

	public class AssetCreateValidationException : OrderCloudIntegrationException
	{
		public AssetCreateValidationException(string message) : base("Asset Create Validation", message, null) { }
	}

	public class DuplicateIDException : OrderCloudIntegrationException
	{
		public DuplicateIDException() : base("IdExists", "Object already exists.", null) { }
	}

	public class SchemaNotValidException : OrderCloudIntegrationException
	{
		public SchemaNotValidException(List<string> errors) : base("Schema Invalid", "Errors with Json Schema", errors) { }
	}

	public class DocumentNotValidException : OrderCloudIntegrationException
	{
		public DocumentNotValidException(string schemaInteropID, List<string> errors) : base("Document Invalid", $"This Document does not conform to schema \"{schemaInteropID}\"", errors) { }
	}

	public class InvalidAssignmentException : OrderCloudIntegrationException
	{
		public InvalidAssignmentException(List<ResourceType> allowed) :
			base("Invalid Assignment", $"This type of document can only be assigned the following resources. This is set on the schema.", allowed.Select(r => Enum.GetName(r.GetType(), r))) { }
	}

	public class InvalidPropertyException : OrderCloudIntegrationException
	{
		public InvalidPropertyException(string model, string error) : base("Invalid Query Param", $"{model} does not contain this property.", error) { }
	}

	public class ValidationException : OrderCloudIntegrationException
	{
		public ValidationException(string field, string message) : base(field, message, null) { }
	}

	public class ReorderImagesOnlyException : OrderCloudIntegrationException
	{
		public ReorderImagesOnlyException() : base("Reorder Error", "Only Image-type assets have an ordering.", null) { }
	}

	public class NotConfiguredForAssetsException : OrderCloudIntegrationException
	{
		public NotConfiguredForAssetsException(string sellerID) : base("Configuration Error", $"Your OC organization with SellerID {sellerID} is not configured to use the Assets feature. Please contact oheywood@four51.com for access.", null) { }
	}

	public class MissingConfigationException : OrderCloudIntegrationException
	{
		public MissingConfigationException() : base("Configuration Error", $"Missing Configuration Record.", null) { }
	}
}
