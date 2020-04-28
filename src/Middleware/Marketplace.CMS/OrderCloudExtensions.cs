﻿using Marketplace.CMS.Models;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.CMS
{
	public static class OrderCloudExtensions
	{
		public static async Task ConfirmExists(this OrderCloudClient oc, ResourceType resourceType, string resourceID, string parentID = null)
		{
			var resource = (OrderCloudResource)oc.GetType().GetProperty(resourceType.ToString()).GetValue(oc);
			var getMethod = resource.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.FirstOrDefault(method => method.Name == "GetAsync" && method.IsGenericMethod == false);
			var paramCount = getMethod.GetParameters().Length;
			var parameters = paramCount == 2 ? new object[] { resourceID, null } : new object[] { parentID, resourceID, null };
			await (Task) getMethod.Invoke(resource, parameters);
		}

		// Want to change to a Get() method, but don't know how to fix this error
		//"Unable to cast object of type 'AsyncStateMachineBox`1[OrderCloud.SDK.Product,Flurl.Http.HttpResponseMessageExtensions+<ReceiveJson>d__0`1[OrderCloud.SDK.Product]]' 
		// to type 'System.Threading.Tasks.Task`1[System.Object]'."
	}
}
