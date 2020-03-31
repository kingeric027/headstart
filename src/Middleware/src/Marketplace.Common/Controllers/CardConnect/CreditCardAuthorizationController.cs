using System.Threading.Tasks;
using Marketplace.Common.Commands;
using Marketplace.Helpers.Attributes;
using Marketplace.Models;
using Marketplace.Models.Attributes;
using Marketplace.Models.Misc;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;

namespace Marketplace.Common.Controllers.CardConnect
{
    [DocComments("\"Integration\" represents ME Credit Card Payments for Marketplace")]
    [MarketplaceSection.Integration(ListOrder = 2)]
    public class MePaymentController : BaseController
    {
        private readonly ICreditCardCommand _card;
        public MePaymentController(AppSettings settings, ICreditCardCommand card) : base(settings)
        {
            _card = card;
        }

        [DocName("POST Payment")]
        [HttpPost, Route("me/payments/{paymentID}"), MarketplaceUserAuth(ApiRole.Shopper)]
        public async Task<Payment> Post(string paymentID, [FromBody] CreditCardPayment payment)
        {
            return await _card.AuthorizePayment(paymentID, payment, VerifiedUserContext);
        }
    }

    [DocComments("\"Integration\" represents ME Credit Card Tokenization for Marketplace")]
    [MarketplaceSection.Integration(ListOrder = 3)]
    public class MeCreditCardAuthorizationController : BaseController
    {
        private readonly ICreditCardCommand _card;
        public MeCreditCardAuthorizationController(AppSettings settings, ICreditCardCommand card) : base(settings)
        {
            _card = card;
        }

        [DocName("POST Credit Card")]
        [HttpPost, Route("me/creditcards"), MarketplaceUserAuth(ApiRole.MeCreditCardAdmin, ApiRole.CreditCardAdmin)]
        public async Task<BuyerCreditCard> MePost([FromBody] CreditCardToken card)
        {
            return await _card.MeTokenizeAndSave(card, this.VerifiedUserContext);
        }
    }

    [DocComments("\"Integration\" represents Credit Card Tokenization for Marketplace")]
    [MarketplaceSection.Integration(ListOrder = 4)]
    public class CreditCardAuthorizationController : BaseController
    {
        private readonly ICreditCardCommand _card;
        public CreditCardAuthorizationController(AppSettings settings, ICreditCardCommand card) : base(settings)
        {
            _card = card;
        }

        [DocName("POST Credit Cards")]
        [HttpPost, Route("buyers/{buyerID}/creditcards"), MarketplaceUserAuth(ApiRole.CreditCardAdmin)]
        public async Task<CreditCard> Post([FromBody] CreditCardToken card, string buyerID)
        {
            return await _card.TokenizeAndSave(buyerID, card, VerifiedUserContext);
        }
    }
}
