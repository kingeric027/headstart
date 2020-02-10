using Marketplace.Common.Commands;
using Marketplace.Common.Models;
using Marketplace.Helpers;
using Marketplace.Helpers.Models;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Controllers
{
    [Route("supplier")]
    public class SupplierController : BaseController
    {
        
        private readonly IMarketplaceSupplierCommand _command;
        public SupplierController(IMarketplaceSupplierCommand command, AppSettings settings) : base(settings)
        {
            _command = command;
        }

        [HttpPost, MarketplaceUserAuth(ApiRole.SupplierAdmin)]
        public async Task<MarketplaceSupplier> Create([FromBody] MarketplaceSupplier supplier)
        {
            return await _command.Create(supplier, VerifiedUserContext);
        } 
    }
}
