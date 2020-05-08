using Marketplace.Common.Commands;
using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Helpers.Attributes;
using Marketplace.Models.Attributes;
using Marketplace.Common.Controllers.CMS;
using Marketplace.CMS.Models;
using Marketplace.CMS.Queries;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Marketplace Suppliers\" represents Supplier in Marketplace")]
    [MarketplaceSection.Marketplace(ListOrder = 2)]
    [Route("suppliers")]
    public class SupplierController: BaseController
    {

		private readonly IMarketplaceSupplierCommand _command;
        private readonly IOrderCloudClient _oc;
        public SupplierController(IMarketplaceSupplierCommand command, IOrderCloudClient oc, AppSettings settings) : base(settings)
        {
            _command = command;
            _oc = oc;
        }

		[DocName("GET MarketplaceSupplier")]
		[HttpGet, Route("me/{supplierID}"), MarketplaceUserAuth(ApiRole.SupplierAdmin, ApiRole.SupplierReader)]
		public async Task<MarketplaceSupplier> GetMySupplier(string supplierID)
		{
			//ocAuth is the token for the organization that is specified in the AppSettings

		   var ocAuth = await _oc.AuthenticateAsync();
			return await _command.GetMySupplier(supplierID, VerifiedUserContext, ocAuth.AccessToken);
		}

		[DocName("POST Marketplace Supplier")]
		[HttpPost, MarketplaceUserAuth(ApiRole.SupplierAdmin)]
		public async Task<MarketplaceSupplier> Create([FromBody] MarketplaceSupplier supplier)
		{
			///ocAuth is the token for the organization that is specified in the AppSettings

		   var ocAuth = await _oc.AuthenticateAsync();
			return await _command.Create(supplier, VerifiedUserContext, ocAuth.AccessToken);
		}
	}
}
