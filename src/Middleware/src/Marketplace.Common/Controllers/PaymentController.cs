using Marketplace.Common.Commands;
using Marketplace.Common.Models;
using Marketplace.Common.Models.Marketplace;
using Marketplace.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using ordercloud.integrations.cardconnect;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Marketplace Orders\" for handling payment commands in Marketplace")]
    [MarketplaceSection.Marketplace(ListOrder = 2)]
    [Route("payments")]
    public class PaymentController : BaseController
    {

        private readonly IPaymentCommand _command;
        private readonly AppSettings _settings;
        public PaymentController(IPaymentCommand command, AppSettings settings) : base(settings)
        {
            _command = command;
            _settings = settings;
        }

        [DocName("Save payments")]
        [DocComments("Creates or updates payments as needed for this order")]
        [HttpPut, Route("{orderID}/update"), OrderCloudIntegrationsAuth(ApiRole.Shopper)]
        public async Task<IList<HSPayment>> SavePayments(string orderID, [FromBody] PaymentUpdateRequest request)
        {
            return await _command.SavePayments(orderID, request.Payments, VerifiedUserContext.AccessToken);
        }
    }
}
