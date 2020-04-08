import { Injectable } from '@angular/core';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { OcTokenService } from '@ordercloud/angular-sdk';
import { AppConfig } from '../../shopper-context';
import { ShipMethodSelection, OrderWorksheet } from './ordercloud-sandbox.models';

// this is a temporary service to represent features that are not yet available in the ordercloud sdk
// currently used for shipping integration routes
@Injectable({
  providedIn: 'root',
})
export class OrderCloudSandboxService {
  baseUrl = ``;
  constructor(private ocTokenService: OcTokenService, private http: HttpClient, public appSettings: AppConfig) {
    // awaiting fully current sdk
    this.baseUrl = `${appSettings.orderCloudApiUrl}/v1`;
  }

  generateHeaders() {
    return {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Authorization: `Bearer ${this.ocTokenService.GetAccess()}`,
      }),
    };
  }

  estimateShipping(orderID: string): Promise<OrderWorksheet> {
    return this.http
      .post<OrderWorksheet>(`${this.baseUrl}/orders/Outgoing/${orderID}/estimateshipping`, {}, this.generateHeaders())
      .toPromise();
  }

  selectShipMethod(orderID: string, selection: ShipMethodSelection): Promise<OrderWorksheet> {
    const requestBody = { ShipmethodSelections: [selection] };
    return this.http
      .post<OrderWorksheet>(
        `${this.baseUrl}/orders/Outgoing/${orderID}/shipmethods`,
        requestBody,
        this.generateHeaders()
      )
      .toPromise();
  }

  calculateOrder(orderID: string): Promise<OrderWorksheet> {
    return this.http
      .post<OrderWorksheet>(`${this.baseUrl}/orders/Outgoing/${orderID}/calculate`, {}, this.generateHeaders())
      .toPromise();
  }
}
