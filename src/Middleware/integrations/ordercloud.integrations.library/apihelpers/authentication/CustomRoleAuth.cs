using Microsoft.Extensions.Localization.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;

namespace ordercloud.integrations.library.apihelpers.authentication
{
	public static class CustomRoleAuth
	{
		public static void RequireOneOf(VerifiedUserContext user, params string[] customRoles)
		{
			var roles = user.Principal.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
			var isAuthenticated = roles.Intersect(customRoles).Count() > 0 || roles.Contains("FullAccess");
			Require.That(isAuthenticated, new ErrorCode("InsufficientRoles", 401, $"Insufficient Roles. One of {string.Join(", ", customRoles)} required."));
		}
	}
}
