import { Injectable } from '@angular/core';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { OcTokenService } from '@ordercloud/angular-sdk';
import { MarketplaceOrder, AppConfig } from '../../shopper-context';
import { ShipmentPreference, OrderCalculation } from './ordercloud-sandbox.models';

// this is a temporary service to represent features that are not yet available in the ordercloud sdk
// currently used for shipping integration routes
@Injectable({
  providedIn: 'root'
})
export class OrderCloudSandboxService {
  readonly baseUrl = `https://sandboxapi.ordercloud.io/v1`
  constructor(
    private ocTokenService: OcTokenService,
    private http: HttpClient,
    public appSettings: AppConfig
  ) {
  }

  generateHeaders() {
    return {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Authorization: `Bearer ${this.ocTokenService.GetAccess()}`
      })
    };
  }

  calculateShippingOptions(orderID: string): Promise<OrderCalculation> {
    return this.http.post<OrderCalculation>(
      `${this.baseUrl}/orders/Outgoing/${orderID}/CalculateShippingOptions`, {}, this.generateHeaders())
    .toPromise();
  }

  selectShippingRate(orderID: string, selection: ShipmentPreference): Promise<OrderCalculation> {
    const requestBody = {ShipmentPreferences: [selection]};
    return this.http.post<OrderCalculation>(
      `${this.baseUrl}/orders/Outgoing/${orderID}/SetShippingPreferences`, requestBody, this.generateHeaders())
    .toPromise();
  } 

  calculateOrder(orderID: string): Promise<OrderCalculation> {
    return this.http.post<OrderCalculation>(
      `${this.baseUrl}/orders/Outgoing/${orderID}/CalculateOrder`, {}, this.generateHeaders())
    .toPromise();
  } 
}
