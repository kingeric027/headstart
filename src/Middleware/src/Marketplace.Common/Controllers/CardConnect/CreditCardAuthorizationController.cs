using System.Threading.Tasks;
using Marketplace.Common.Commands;
using Marketplace.Models;
using Marketplace.Models.Attributes;
using Marketplace.Models.Misc;
using Microsoft.AspNetCore.Mvc;
using ordercloud.integrations.cardconnect;
using ordercloud.integrations.extensions;
using ordercloud.integrations.openapispec;
using OrderCloud.SDK;

namespace Marketplace.Common.Controllers.CardConnect
{
    [DocComments("\"Integration\" represents ME Credit Card Payments for Marketplace")]
    [MarketplaceSection.Integration(ListOrder = 2)]
    public class MePaymentController : BaseController
    {
        private readonly IOrderCloudIntegrationsCardConnectCommand _card;
        public MePaymentController(AppSettings settings, IOrderCloudIntegrationsCardConnectCommand card) : base(settings)
        {
            _card = card;
        }

        [DocName("POST Payment")]
        [HttpPost, Route("me/payments"), OrderCloudIntegrationsAuth(ApiRole.Shopper)]
        public async Task<Payment> Post([FromBody] OrderCloudIntegrationsCreditCardPayment payment)
        {
            return await _card.AuthorizePayment(payment, VerifiedUserContext);
        }
    }

    [DocComments("\"Integration\" represents ME Credit Card Tokenization for Marketplace")]
    [MarketplaceSection.Integration(ListOrder = 3)]
    public class MeCreditCardAuthorizationController : BaseController
    {
        private readonly IOrderCloudIntegrationsCardConnectCommand _card;
        public MeCreditCardAuthorizationController(AppSettings settings, IOrderCloudIntegrationsCardConnectCommand card) : base(settings)
        {
            _card = card;
        }

        [DocName("POST Credit Card")]
        [HttpPost, Route("me/creditcards"), OrderCloudIntegrationsAuth(ApiRole.MeCreditCardAdmin, ApiRole.CreditCardAdmin)]
        public async Task<BuyerCreditCard> MePost([FromBody] OrderCloudIntegrationsCreditCardToken card)
        {
            return await _card.MeTokenizeAndSave(card, this.VerifiedUserContext);
        }
    }

    [DocComments("\"Integration\" represents Credit Card Tokenization for Marketplace")]
    [MarketplaceSection.Integration(ListOrder = 4)]
    public class CreditCardAuthorizationController : BaseController
    {
        private readonly IOrderCloudIntegrationsCardConnectCommand _card;
        public CreditCardAuthorizationController(AppSettings settings, IOrderCloudIntegrationsCardConnectCommand card) : base(settings)
        {
            _card = card;
        }

        [DocName("POST Credit Cards")]
        [HttpPost, Route("buyers/{buyerID}/creditcards"), OrderCloudIntegrationsAuth(ApiRole.CreditCardAdmin)]
        public async Task<CreditCard> Post([FromBody] OrderCloudIntegrationsCreditCardToken card, string buyerID)
        {
            return await _card.TokenizeAndSave(buyerID, card, VerifiedUserContext);
        }
    }
}
