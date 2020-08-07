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
		[RequireBasedOnType]
		public string ParentResourceID { get; set; } = null;
		[Required]
		public ResourceType? ResourceType { get; set; }
	}

	public class RequireBasedOnTypeAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var instance = validationContext.ObjectInstance;
			var type = (ResourceType) instance.GetType().GetProperty("ResourceType").GetValue(instance);
			var hasParentType = type.GetType().HasAttribute<ParentTypeAttribute>();
			if (hasParentType && value == null) return new ValidationResult("ParentResourceID is required.");
			return ValidationResult.Success;
		}

	}
}
