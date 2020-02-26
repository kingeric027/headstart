using Marketplace.Common.Commands;
using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Helpers.SwaggerTools;
using Marketplace.Models.Attributes;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Suppliers\" represents Supplier in Marketplace")]
    [MarketplaceSection.Suppliers(ListOrder = 1)]
    [Route("supplier")]
    public class SupplierController : BaseController
    {
        
        private readonly IMarketplaceSupplierCommand _command;
        private readonly IOrderCloudClient _oc;
        public SupplierController(IMarketplaceSupplierCommand command, IOrderCloudClient oc, AppSettings settings) : base(settings)
        {
            _command = command;
            _oc = oc;
        }

        [DocName("POST Supplier")]
        [HttpPost, MarketplaceUserAuth(ApiRole.SupplierAdmin)]
        public async Task<MarketplaceSupplier> Create([FromBody] MarketplaceSupplier supplier)
        {
            // ocAuth is the token for the organization that is specified in the AppSettings
            var ocAuth = await _oc.AuthenticateAsync();
            return await _command.Create(supplier, VerifiedUserContext, ocAuth.AccessToken);
        } 
    }
}
