import { Injectable } from '@angular/core';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { OcTokenService, BuyerCreditCard, Payment } from '@ordercloud/angular-sdk';
import { SupplierCategoryConfig, MarketplaceOrder, AppConfig, ProposedShipment, ProposedShipmentSelection, CreditCardToken } from '../../shopper-context';

@Injectable({
  providedIn: 'root'
})
export class MiddlewareApiService {
  readonly baseUrl: string;
  constructor(
    private ocTokenService: OcTokenService,
    private http: HttpClient,
    public appSettings: AppConfig
  ) {
    this.baseUrl = this.appSettings.middlewareUrl;
  }

  generateHeaders() {
    return {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Authorization: `Bearer ${this.ocTokenService.GetAccess()}`
      })
    };
  }

  getMarketplaceSupplierCategories(
    marketplaceID: string
  ): Promise<SupplierCategoryConfig> {
    return this.http
      .get<SupplierCategoryConfig>(
        `${this.baseUrl}/marketplace/${marketplaceID}/supplier/category/config`,
        this.generateHeaders()
      )
      .toPromise();
  }

  getProposedShipments(orderID: string): Promise<ProposedShipment[]> {
    return this.http.get<ProposedShipment[]>(
      `${this.baseUrl}/proposedshipment/${orderID}`, this.generateHeaders())
    .toPromise();
  }

  selectShippingRate(orderID: string, selection: ProposedShipmentSelection): Promise<MarketplaceOrder> {
    return this.http.put<MarketplaceOrder>(
      `${this.baseUrl}/proposedshipment/${orderID}/select`, selection, this.generateHeaders())
    .toPromise();
  }

  calculateTax(orderID: string): Promise<MarketplaceOrder> {
    return this.http
      .post<MarketplaceOrder>(
        `${this.baseUrl}/orders/${orderID}/tax-transaction`,
        {},
        this.generateHeaders()
      )
      .toPromise();
  }

  saveMeCreditCard(card: CreditCardToken): Promise<BuyerCreditCard> {
    return this.http.post<MarketplaceOrder>(`${this.baseUrl}/me/creditcards`, card, this.generateHeaders())
      .toPromise();
  }

  authOnlyCreditCard(orderID: string, card: CreditCardToken, cvv: string): Promise<Payment> {
    const ccPayment = {
      OrderId: orderID,
      CreditCardDetails: card,
      Currency: 'USD',
      CVV: cvv,
      MerchantID: this.appSettings.cardConnectMerchantID
    };
    return this.http.post<Payment>(`${this.baseUrl}/me/payments`, ccPayment, this.generateHeaders())
      .toPromise();
  }

  authOnlySavedCreditCard(orderID: string, creditCardID: string, cvv: string): Promise<Payment> {
    const ccPayment = {
      OrderId: orderID,
      CreditCardID: creditCardID,
      Currency: 'USD',
      CVV: cvv,
      MerchantID: this.appSettings.cardConnectMerchantID
    };
    return this.http.post<Payment>(`${this.baseUrl}/me/payments`, ccPayment, this.generateHeaders())
      .toPromise();
  }
}
