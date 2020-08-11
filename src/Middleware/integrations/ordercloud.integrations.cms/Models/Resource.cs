using ordercloud.integrations.library;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace ordercloud.integrations.cms
{
	[SwaggerModel]
	public class Resource
	{
		public Resource() { }
		public Resource(ResourceType type, string id, string parentID = null)
		{
			ResourceType = type;
			ResourceID = id;
			ParentResourceID = parentID;
		}
		[Required]
		public string ResourceID { get; set; }
		[Required]
		public ResourceType? ResourceType { get; set; }
		[RequireBasedOnResourceType]
		public string ParentResourceID { get; set; } = null;

	}

	public class RequireBasedOnResourceTypeAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var instance = validationContext.ObjectInstance;
			var resourceType = instance.GetType().GetProperty("ResourceType").GetValue(instance);
			if (resourceType == null) return ValidationResult.Success;
			var field = typeof(ResourceType).GetField(resourceType.ToString());
			var parentType = field.GetAttribute<ParentResourceAttribute>();
			if (parentType != null && value == null) return new ValidationResult($"ParentResourceID is required. For resource {resourceType.ToString()} please supply a {parentType.ParentType} ID");
			return ValidationResult.Success;
		}
	}
}
