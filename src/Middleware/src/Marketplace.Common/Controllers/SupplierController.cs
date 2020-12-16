using Marketplace.Common.Commands;
using Marketplace.Models.Models.Marketplace;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Models.Attributes;
using ordercloud.integrations.library;
using Marketplace.Models;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Marketplace Suppliers\" represents Supplier in Marketplace")]
    [MarketplaceSection.Marketplace(ListOrder = 2)]
    [Route("supplier")]
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
		[HttpGet, Route("me/{supplierID}"), OrderCloudIntegrationsAuth]
		public async Task<MarketplaceSupplier> GetMySupplier(string supplierID)
		{
			return await _command.GetMySupplier(supplierID, VerifiedUserContext);
		}

		[DocName("POST Marketplace Supplier")]
		[HttpPost, OrderCloudIntegrationsAuth(ApiRole.SupplierAdmin)]
		public async Task<MarketplaceSupplier> Create([FromBody] MarketplaceSupplier supplier)
		{
			return await _command.Create(supplier, VerifiedUserContext);
		}

		[DocName("GET If Location Deletable")]
		[HttpGet, Route("candelete/{locationID}"), OrderCloudIntegrationsAuth(ApiRole.SupplierAddressAdmin)]
		public async Task<bool> CanDeleteLocation(string locationID)
		{
			var productList = await _oc.Products.ListAsync(filters: $"ShipFromAddressID={locationID}");
			return productList.Items.Count == 0;
		}

		[DocName("PATCH Supplier")]
		[DocIgnore] // PartialSupplier throws an openapi error?
		[HttpPatch, Route("{supplierID}"), OrderCloudIntegrationsAuth]
		public async Task<MarketplaceSupplier> UpdateSupplier(string supplierID, [FromBody] PartialSupplier supplier)
		{
			return await _command.UpdateSupplier(supplierID, supplier, VerifiedUserContext);
		}

		[DocName("GET Supplier Order Details")]
		[HttpGet, Route("orderdetails/{supplierOrderID}"), OrderCloudIntegrationsAuth(ApiRole.OrderAdmin, ApiRole.OrderReader)]
		public async Task<MarketplaceSupplierOrderData> GetSupplierOrder(string supplierOrderID)
        {
			return await _command.GetSupplierOrderData(supplierOrderID, VerifiedUserContext);
        }

	}
}
