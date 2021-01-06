using Common;
using Common.Services.AnytimeDashboard;
using Flurl.Http;
using Marketplace.Common.Mappers;
using Marketplace.Common.Services.WazingDashboard;
using Marketplace.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Models
{
	public enum FranchiseEnum
	{
		AnytimeFitness,
		WaxingTheCity
	}

	public static class FranchiseExtensions
	{
		public static IFranchiseAPI GetIFranchise(this FranchiseEnum franchise, AppSettings settings)
		{
			return franchise switch
			{
				FranchiseEnum.AnytimeFitness => new AnytimeDashboardClient(settings),
				FranchiseEnum.WaxingTheCity => new WaxDashboardClient(settings),
				_ => null,
			};
		}
	}

	public interface IFranchiseAPI
	{
		string BuyerID { get;  }
		string BrandName { get; }
		string StorefrontClientID { get; }
		string StorefrontUrl { get; }
		Task ProcessAllLocations(Func<SyncLocation, Task> proccessLocation);
		Task<IEnumerable<SyncUser>> ListAllUsersOnLocation(SyncLocation location);
		List<string> GetOrderCloudCatalogsIDs(SyncLocation location);
		Task<SSOAuthFields> AuthenticateWithCode(string code);
		string BuildAuthorizeUrl(string redirectHost, string state);
	}

	public class FranchiseAPIException : FlurlHttpException
	{ 
		public JObject Response { get; }
		public FranchiseAPIException(string response, FlurlHttpException ex) : base(ex.Call, $"{ex.Message}. Body: {response}", ex.InnerException)
		{
			Response = JObject.Parse(response);
		}
	}
}
