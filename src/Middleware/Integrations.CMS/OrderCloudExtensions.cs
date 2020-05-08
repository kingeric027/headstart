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


		// Used to confirm resource exists and user has READ access
		public static async Task Get(this OrderCloudClient oc, Resource resource)
		{
			var sdk = (OrderCloudResource)oc.GetType().GetProperty(resource.Type.ToString()).GetValue(oc);
			var getMethod = sdk.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.FirstOrDefault(method => method.Name == "GetAsync" && method.IsGenericMethod == false);
			var paramCount = getMethod.GetParameters().Length;
			var parameters = paramCount == 2 ? new object[] { resource.ID, null } : new object[] { resource.ParentID, resource.ID, null };
			try
			{
				await (Task)getMethod.Invoke(sdk, parameters);
			} catch (Exception ex)
			{
				throw new TokenExpiredException(); // TODO - this is really a bug with MultiTenantOCClient() 
			}
		}

		// Used to confirm resource exists and user has WRITE access
		public static async Task EmptyPatch(this OrderCloudClient oc, Resource resource)
		{
			var sdk = (OrderCloudResource)oc.GetType().GetProperty(resource.Type.ToString()).GetValue(oc);
			var getMethod = sdk.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.FirstOrDefault(method => method.Name == "PatchAsync" && method.IsGenericMethod == false);
			var paramInfo = getMethod.GetParameters();
			var emptyPatch = Activator.CreateInstance(paramInfo[paramInfo.Length - 2].ParameterType);
			var parameters = paramInfo.Length == 3 ? new object[] { resource.ID, emptyPatch, null } : new object[] { resource.ParentID, resource.ID, emptyPatch, null };
			try
			{
				await (Task)getMethod.Invoke(sdk, parameters);
			} catch (Exception ex)
			{
				throw new TokenExpiredException(); // TODO - this is really a bug with MultiTenantOCClient() 
			}
		}
	}
}
