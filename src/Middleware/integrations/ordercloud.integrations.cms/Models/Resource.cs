using ordercloud.integrations.library;
using ordercloud.integrations.library.Cosmos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace ordercloud.integrations.cms
{
	[SwaggerModel]
	public class DocumentAssignment : Resource
	{
		[Required]
		public string DocumentID { get; set; }
	}

	[SwaggerModel]
	public class AssetAssignment : Resource
	{
		[Required]
		public string AssetID { get; set; }
	}

	[SwaggerModel]
	public class Resource
	{
		public Resource() { }
		public Resource(ResourceType type, string id)
		{
			ResourceType = type;
			ResourceID = id;
		}

		public Resource(ResourceType type, string id, ParentResourceType parentType, string parentID)
		{
			ResourceType = type;
			ResourceID = id;
			ParentResourceID = parentID;
			ParentResourceType = parentType;
		}

		[Required]
		public string ResourceID { get; set; }
		[Required]
		public ResourceType? ResourceType { get; set; }
		public string ParentResourceID { get; set; } = null;
		public ParentResourceType? ParentResourceType { get; set; }

		public void Validate()
		{
			var correctParentType = ResourceType?.GetParentType();
			if (correctParentType != null)
			{
				if (ParentResourceType != correctParentType)
				{
					throw new ValidationException($"Field ParentResourceType must be {ParentResourceType} for type ${ResourceType}.");
				}
				if (ParentResourceID == null)
				{
					throw new ValidationException($"Field ParentResourceID is required for type ${ResourceType}.");
				}
			}
			else
			{
				if (ParentResourceType != null)
				{
					throw new ValidationException($"Field ParentResourceType must be null for type ${ResourceType}.");
				}
				if (ParentResourceID != null)
				{
					throw new ValidationException($"Field ParentResourceID must be null for type ${ResourceType}.");
				}
			}
		}
	}
}
