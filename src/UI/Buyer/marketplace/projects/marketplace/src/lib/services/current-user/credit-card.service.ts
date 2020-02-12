import { Injectable } from '@angular/core';
import { BuyerCreditCard, ListBuyerCreditCard, OcMeService } from '@ordercloud/angular-sdk';
import { CreditCardToken } from '../../shopper-context';
import { MiddlewareApiService } from '../middleware-api/middleware-api.service';

export interface ICreditCards {
  Save(card: CreditCardToken): Promise<BuyerCreditCard>;
  Delete(cardID: string): Promise<void>;
  List(): Promise<ListBuyerCreditCard>;
}

@Injectable({
  providedIn: 'root',
})
export class CreditCardService implements ICreditCards {
  private readonly cardTypes = {
    Visa: RegExp('^4[0-9]{12}(?:[0-9]{3})?$'), // e.g. 4000000000000000
    MasterCard: RegExp('^5[1-5][0-9]{14}$'), // e.g. 5100000000000000
    Discover: RegExp('^6(?:011|5[0-9]{2})[0-9]{12}$'), // e.g. 6011000000000000
    Amex: RegExp('^3[47][0-9]{13}$'),
    BCGlobal: RegExp('^(6541|6556)[0-9]{12}$'),
    DinersClub: RegExp('^3(?:0[0-5]|[68][0-9])[0-9]{11}$'),
    JCB: RegExp('^(?:2131|1800|35\d{3})\d{11}$'),
    UnionPay: RegExp('^(62[0-9]{14,17})$'),
  };

  constructor(private ocMeService: OcMeService, private middleware: MiddlewareApiService) {}

  async Save(card: CreditCardToken): Promise<BuyerCreditCard> {
    card.CardType = this.getCardType(card.AccountNumber);
    return await this.middleware.saveMeCreditCard(card);
  }

  async Delete(cardID: string): Promise<void> {
    return await this.ocMeService.DeleteCreditCard(cardID).toPromise();
  }

  async List(): Promise<ListBuyerCreditCard> {
    return await this.ocMeService.ListCreditCards({ pageSize: 100 }).toPromise();
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
