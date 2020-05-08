using Marketplace.CMS.Models;
using Marketplace.Helpers.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.CMS
{
	public class StorageConnectionException : ApiErrorException
	{
		public StorageConnectionException(string containerInteropID, object ex) : base("Storage Connection", 400, $"Container {containerInteropID} storage connection failed.", ex) { }
	}

	public class AssetUploadValidationException : ApiErrorException
	{
		public AssetUploadValidationException(string message) : base("Asset Upload Validation", 400, message, null) { }
	}

	public class TokenExpiredException : ApiErrorException
	{
		public TokenExpiredException() : base("Token", 400, "Token has expired", null) { }
	}
}
