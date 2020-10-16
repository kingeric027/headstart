using System.Threading.Tasks;
using Marketplace.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using ordercloud.integrations.cardconnect;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Common.Controllers.CardConnect
{
    [DocComments("\"Integration\" represents ME Credit Card Payments for Marketplace")]
    [MarketplaceSection.Integration(ListOrder = 2)]
    public class MePaymentController : BaseController
    {
        private readonly IOrderCloudIntegrationsCardConnectCommand _card;
        private readonly AppSettings _settings;
        public MePaymentController(AppSettings settings, IOrderCloudIntegrationsCardConnectCommand card) : base(settings)
        {
            _card = card;
            _settings = settings;
        }

        [DocName("POST Payment")]
        [HttpPost, Route("me/payments"), OrderCloudIntegrationsAuth(ApiRole.Shopper)]
        public async Task<Payment> Post([FromBody] OrderCloudIntegrationsCreditCardPayment payment)
        {
            string merchantID;
            if (payment.Currency == "USD")
                merchantID = _settings.CardConnectSettings.UsdMerchantID;
            else if (payment.Currency == "CAD")
                merchantID = _settings.CardConnectSettings.CadMerchantID;
            else
                merchantID = _settings.CardConnectSettings.EurMerchantID;
                
            return await _card.AuthorizePayment(payment, VerifiedUserContext, merchantID);
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
