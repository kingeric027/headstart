using Integrations.CMS.Models;
using Marketplace.CMS.Models;
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
		// I Want to change this to a Get() method, but when I tried the line below there's an error I don't understand
		// await (Task<object>) getMethod.Invoke(resource, parameters);
		//
		//"Unable to cast object of type 'AsyncStateMachineBox`1[OrderCloud.SDK.Product,Flurl.Http.HttpResponseMessageExtensions+<ReceiveJson>d__0`1[OrderCloud.SDK.Product]]' 
		// to type 'System.Threading.Tasks.Task`1[System.Object]'."

		public static async Task Get(this OrderCloudClient oc, ResourceType resourceType, string resourceID, string parentID = null)
		{
			var resource = (OrderCloudResource)oc.GetType().GetProperty(resourceType.ToString()).GetValue(oc);
			var getMethod = resource.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.FirstOrDefault(method => method.Name == "GetAsync" && method.IsGenericMethod == false);
			var paramCount = getMethod.GetParameters().Length;
			var parameters = paramCount == 2 ? new object[] { resourceID, null } : new object[] { parentID, resourceID, null };
			await (Task)getMethod.Invoke(resource, parameters);
		}

		// Object of type '<>f__AnonymousType2' cannot be converted to type 'OrderCloud.SDK.PartialCatalog
		//public static async Task EmptyPatch(this OrderCloudClient oc, ResourceType resourceType, string resourceID, string parentID = null)
		//{
		//	var resource = (OrderCloudResource)oc.GetType().GetProperty(resourceType.ToString()).GetValue(oc);
		//	var getMethod = resource.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
		//		.FirstOrDefault(method => method.Name == "PatchAsync" && method.IsGenericMethod == false);
		//	var paramCount = getMethod.GetParameters().Length;
		//	var parameters = paramCount == 3 ? new object[] { resourceID, new { }, null } : new object[] { parentID, resourceID, new { }, null };
		//	await (Task)getMethod.Invoke(resource, parameters);
		//}
	}
}
