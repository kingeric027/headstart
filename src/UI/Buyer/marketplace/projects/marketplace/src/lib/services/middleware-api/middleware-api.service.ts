import { Injectable } from '@angular/core';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { OcTokenService, BuyerCreditCard, Payment } from '@ordercloud/angular-sdk';
import {
  SupplierCategoryConfig,
  MarketplaceOrder,
  AppConfig,
  CreditCardToken,
  CreditCardPayment,
} from '../../shopper-context';

@Injectable({
  providedIn: 'root',
})
export class MiddlewareApiService {
  readonly baseUrl: string;
  constructor(private ocTokenService: OcTokenService, private http: HttpClient, public appSettings: AppConfig) {
    this.baseUrl = this.appSettings.middlewareUrl;
  }

  generateHeaders() {
    return {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Authorization: `Bearer ${this.ocTokenService.GetAccess()}`,
      }),
    };
  }

  getMarketplaceSupplierCategories(marketplaceID: string): Promise<SupplierCategoryConfig> {
    return this.http
      .get<SupplierCategoryConfig>(
        `${this.baseUrl}/marketplace/${marketplaceID}/supplier/category/config`,
        this.generateHeaders()
      )
      .toPromise();
  }

  saveMeCreditCard(card: CreditCardToken): Promise<BuyerCreditCard> {
    return this.http.post<MarketplaceOrder>(`${this.baseUrl}/me/creditcards`, card, this.generateHeaders()).toPromise();
  }

  authorizeCreditCard(orderID: string, payment: CreditCardPayment): Promise<Payment> {
    const ccPayment = {
      OrderId: orderID,
      CreditCardID: payment?.SavedCard.ID,
      CreditCardDetails: payment.NewCard,
      Currency: 'USD',
      CVV: payment.CVV,
      MerchantID: this.appSettings.cardConnectMerchantID,
    };
    return this.http.post<Payment>(`${this.baseUrl}/me/payments`, ccPayment, this.generateHeaders()).toPromise();
  }
}
