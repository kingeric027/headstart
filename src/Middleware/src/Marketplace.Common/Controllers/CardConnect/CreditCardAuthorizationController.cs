using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Marketplace.Common.Commands.CardConnect;
using Marketplace.Common.Models.CardConnect;
using Marketplace.Helpers;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;

namespace Marketplace.Common.Controllers.CardConnect
{
    public class MePaymentController : BaseController
    {
        private readonly ICreditCardCommand _card;
        public MePaymentController(AppSettings settings, ICreditCardCommand card) : base(settings)
        {
            _card = card;
        }

        [HttpPost, Route("me/payments"), MarketplaceUserAuth(ApiRole.Shopper)]
        public async Task<Payment> Post([FromBody] CreditCardPayment payment)
        {
            return await _card.AuthorizePayment(payment, VerifiedUserContext);
        }
    }

    public class MeCreditCardAuthorizationController : BaseController
    {
        private readonly ICreditCardCommand _card;
        public MeCreditCardAuthorizationController(AppSettings settings, ICreditCardCommand card) : base(settings)
        {
            _card = card;
        }

        [HttpPost, Route("me/creditcards"), MarketplaceUserAuth(ApiRole.MeCreditCardAdmin, ApiRole.CreditCardAdmin)]
        public async Task<BuyerCreditCard> MePost([FromBody] CreditCardToken card)
        {
            return await _card.MeTokenizeAndSave(card, this.VerifiedUserContext);
        }
    }

    public class CreditCardAuthorizationController : BaseController
    {
        private readonly ICreditCardCommand _card;
        public CreditCardAuthorizationController(AppSettings settings, ICreditCardCommand card) : base(settings)
        {
            _card = card;
        }

        [HttpPost, Route("buyers/{buyerID}/creditcards"), MarketplaceUserAuth(ApiRole.CreditCardAdmin)]
        public async Task<CreditCard> Post([FromBody] CreditCardToken card, string buyerID)
        {
            return await _card.TokenizeAndSave(buyerID, card, VerifiedUserContext);
        }
    }
}
