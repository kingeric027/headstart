import { Injectable } from '@angular/core';
import { Me, ListPage } from 'ordercloud-javascript-sdk';
import { HeadStartSDK, OrderCloudIntegrationsCreditCardToken } from '@ordercloud/headstart-sdk';
import { MarketplaceBuyerCreditCard } from '../../shopper-context';

@Injectable({
  providedIn: 'root',
})
export class CreditCardService {
  private readonly cardTypes = {
    Visa: RegExp('^4[0-9]{12}(?:[0-9]{3})?$'), // e.g. 4000000000000000
    MasterCard: RegExp('^5[1-5][0-9]{14}$'), // e.g. 5100000000000000
    Discover: RegExp('^6(?:011|5[0-9]{2})[0-9]{12}$'), // e.g. 6011000000000000
    Amex: RegExp('^3[47][0-9]{13}$'),
    BCGlobal: RegExp('^(6541|6556)[0-9]{12}$'),
    DinersClub: RegExp('^3(?:0[0-5]|[68][0-9])[0-9]{11}$'),
    JCB: RegExp(`^(?:2131|1800|35\d{3})\d{11}$`),
    UnionPay: RegExp('^(62[0-9]{14,17})$'),
  };

  constructor() {}

  async Save(card: OrderCloudIntegrationsCreditCardToken): Promise<MarketplaceBuyerCreditCard> {
    card.CardType = this.getCardType(card.AccountNumber);
    return await HeadStartSDK.MeCreditCardAuthorizations.MePost(card);
  }

  async Delete(cardID: string): Promise<void> {
    return await Me.DeleteCreditCard(cardID);
  }

  async List(): Promise<ListPage<MarketplaceBuyerCreditCard>> {
    return await Me.ListCreditCards({ pageSize: 100 });
  }

  private getCardType(cardNumber: string): string {
    if (!cardNumber) return null;

    for (const type in this.cardTypes) {
      if (this.cardTypes.hasOwnProperty(type)) {
        if (this.cardTypes[type].test(cardNumber)) {
          return type;
        }
      }
    }
    return null;
  }
}
