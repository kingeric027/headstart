import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { applicationConfiguration } from 'src/app/config/app.config';
import { BuyerCreditCard } from '@ordercloud/angular-sdk';
import { AuthService } from '../auth/auth.service';
import { AppConfig, AuthNetCreditCard, ICreditCards, CreateCardResponse } from 'shopper-context-interface';

/**
 *  OrderCloud does not store full credit card details or process finacial transactions.
 *  An integration with a third party service is required for these features.
 *  This service is an example of such an integration using Authorize.net
 *  (https://developer.authorize.net/api/reference/index.html)
 *
 *  Note: we strongly recommend doing such integrations server-side. Eventually, a server-side example may be added.
 * */

@Injectable({
  providedIn: 'root',
})
export class AuthNetCreditCardService implements ICreditCards {
  readonly url = 'https://api.ordercloud.io/v1/integrationproxy/authorizenet';
  readonly options = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.authService.getOCToken()}`,
    }),
  };
  readonly acceptedCards = {
    Visa: RegExp('^4[0-9]{12}(?:[0-9]{3})?$'), // e.g. 4000000000000000
    MasterCard: RegExp('^5[1-5][0-9]{14}$'), // e.g. 5100000000000000
    Discover: RegExp('^6(?:011|5[0-9]{2})[0-9]{12}$'), // e.g. 6011000000000000
  };

  constructor(private http: HttpClient, private authService: AuthService, @Inject(applicationConfiguration) private appConfig: AppConfig) {}

  private post(body): Promise<any> {
    return this.http.post<any>(this.url, body, this.options).toPromise();
  }

  CreateSavedCard(card: AuthNetCreditCard): Promise<CreateCardResponse> {
    return this.post({
      BuyerID: this.appConfig.appname,
      TransactionType: 'createCreditCard',
      CardDetails: {
        CardType: this.getCardType(card.CardNumber),
        ...card,
        Shared: false,
      },
    });
  }

  DeleteSavedCard(cardID: string): Promise<void> {
    return this.post({
      BuyerID: this.appConfig.appname,
      TransactionType: 'deleteCreditCard',
      CardDetails: {
        CreditCardID: cardID,
        Shared: false,
      },
    });
  }

  // private AuthorizeCardOnOrder(order: Order, card: AuthNetCreditCard): Promise<AuthorizeCardSuccess> {
  //   return this.post({
  //     BuyerID: this.appConfig.appname,
  //     OrderID: order.ID,
  //     OrderDirection: 'outgoing',
  //     Amount: order.Total,
  //     TransactionType: 'authOnlyTransaction',
  //     CardDetails: {
  //       ...card,
  //       Shared: false,
  //     },
  //   });
  // }

  /**
   * The CaptureTransaction() method should remain commented out until Jeff confirms that the OC Auth.net integration is production-ready.
   *

  CaptureTransaction(orderId: string, paymentId: string) {
    return this.post({
      BuyerID: this.appConfig.appname,
      OrderID: orderId,
      OrderDirection: 'outgoing',
      TransactionType: 'priorAuthCaptureTransaction',
      CardDetails: {
        PaymentID: paymentId
      }
    });
  }

  */

  /**
    The OC API integration with Authorize.Net is still working through some bugs. One involves existing Payments.
    This function wraps AuthorizeCardOnOrder() with the calls needed to work around this bug.
    TODO - existingPayment should not be a parameter to this function
  */
  // async AuthorizeCard(order: Order, card: AuthNetCreditCard, existingPayment: Payment): Promise<AuthorizeCardSuccess> {
  //   if (existingPayment) {
  //     await this.ocPaymentService.Delete('outgoing', order.ID, existingPayment.ID).toPromise();
  //   }
  //   return this.AuthorizeCardOnOrder(order, card);
  // }

  /**
    Use AuthorizeAnonymousCard() for anonymous shoppers.
    These cards' non-sensitive details are not saved to a user, because no user exists.
    Instead they must be saved on the order xp.
    TODO - I think this should be saved to payment xp?
  */
  // async AuthorizeAnonymousCard(order: Order, card: AuthNetCreditCard, existingPayment: Payment): Promise<Order> {
  //   await this.AuthorizeCard(order, card, existingPayment);
  //   const patch = { xp: { cardDetails: this.mapToNonSensitive(card) } };
  //   return this.ocOrderService.Patch('outgoing', order.ID, patch).toPromise();
  // }

  getCardType(cardNumber: string): string {
    if (!cardNumber) {
      return null;
    }

    for (const type in this.acceptedCards) {
      if (this.acceptedCards.hasOwnProperty(type)) {
        if (this.acceptedCards[type].test(cardNumber)) {
          return type;
        }
      }
    }
    throw Error('Card number does not match accepted credit card companies');
  }

  mapToNonSensitive(card: AuthNetCreditCard): BuyerCreditCard {
    return {
      CardType: this.getCardType(card.CardNumber),
      PartialAccountNumber: card.CardNumber.slice(-4),
      CardholderName: card.CardholderName,
      ExpirationDate: card.ExpirationDate,
    };
  }
}