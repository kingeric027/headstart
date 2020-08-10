import { Injectable } from '@angular/core';
import { Orders, LineItems, Me, Suppliers, SupplierAddresses, Tokens } from 'ordercloud-javascript-sdk';
import { ReorderHelperService } from '../reorder/reorder.service';
import { OrderReorderResponse, LineItemGroupSupplier, AppConfig } from '../../shopper-context';
import { OrderFilterService } from './order-filter.service';
import {
  MarketplaceAddressBuyer,
  MarketplaceOrder,
  MarketplaceLineItem,
  OrderDetails,
  MarketplaceShipmentWithItems,
  HeadStartSDK,
} from '@ordercloud/headstart-sdk';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { ClaimStatus, LineItemStatus } from '../../../lib/shopper-context';
import { TempSdk } from '../temp-sdk/temp-sdk.service';
@Injectable({
  providedIn: 'root',
})
export class OrderHistoryService {
  activeOrderID: string; // TODO - make this read-only in components

  constructor(
    public filters: OrderFilterService,
    private reorderHelper: ReorderHelperService,
    private httpClient: HttpClient,
    private appConfig: AppConfig,
    private tempSdk: TempSdk
  ) {}

  async getLocationsUserCanView(): Promise<MarketplaceAddressBuyer[]> {
    // add strong type when sdk is regenerated
    const accessUserGroups = await Me.ListUserGroups({ filters: { 'xp.Role': 'ViewAllOrders' } });
    const locationRequests = accessUserGroups.Items.map(a => Me.GetAddress(a.xp.Location));
    const locationResponses = await Promise.all(locationRequests);
    return locationResponses;
  }

  async approveOrder(
    orderID: string = this.activeOrderID,
    Comments: string = '',
    AllowResubmit: boolean = false
  ): Promise<MarketplaceOrder> {
    const order = await Orders.Approve('Outgoing', orderID, { Comments, AllowResubmit });
    return order as MarketplaceOrder;
  }

  async declineOrder(
    orderID: string = this.activeOrderID,
    Comments: string = '',
    AllowResubmit: boolean = true
  ): Promise<MarketplaceOrder> {
    const order = await Orders.Decline('Outgoing', orderID, { Comments, AllowResubmit });
    return order as MarketplaceOrder;
  }

  async validateReorder(
    orderID: string = this.activeOrderID,
    lineItems: MarketplaceLineItem[]
  ): Promise<OrderReorderResponse> {
    return this.reorderHelper.validateReorder(orderID, lineItems);
  }

  async getOrderDetails(orderID: string = this.activeOrderID): Promise<OrderDetails> {
    return await HeadStartSDK.Orders.GetOrderDetails(orderID);
  }

  async getLineItemSuppliers(liGroups: MarketplaceLineItem[][]): Promise<LineItemGroupSupplier[]> {
    const suppliers: LineItemGroupSupplier[] = [];
    for (const group of liGroups) {
      const line = group[0];
      if (line?.SupplierID) {
        const supplier = await Suppliers.Get(line.SupplierID);
        const shipFrom = await SupplierAddresses.Get(line.SupplierID, line.ShipFromAddressID);
        suppliers.push({ supplier, shipFrom });
      }
    }
    return suppliers;
  }

  async listShipments(orderID: string = this.activeOrderID): Promise<MarketplaceShipmentWithItems[]> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${Tokens.GetAccessToken()}`,
    });
    const url = `${this.appConfig.middlewareUrl}/order/${orderID}/shipmentswithitems`;
    return this.httpClient
      .get<MarketplaceShipmentWithItems[]>(url, { headers })
      .toPromise();
  }

  async submitCancelOrReturn(orderID: string, lineItemStatusChange: any): Promise<MarketplaceLineItem[]> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${Tokens.GetAccessToken()}`,
    });
    const url = `${this.appConfig.middlewareUrl}/order/${orderID}/lineitem/status`;
    return this.httpClient
      .post<MarketplaceLineItem[]>(url, lineItemStatusChange, { headers })
      .toPromise();
  }
}
