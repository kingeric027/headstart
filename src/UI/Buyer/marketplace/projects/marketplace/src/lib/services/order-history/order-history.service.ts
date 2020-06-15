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
  MarketplaceSDK,
} from 'marketplace-javascript-sdk';
import { HttpHeaders, HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class OrderHistoryService {
  activeOrderID: string; // TODO - make this read-only in components

  constructor(
    public filters: OrderFilterService,
    private reorderHelper: ReorderHelperService,
    private httpClient: HttpClient,
    private appConfig: AppConfig
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
    return await MarketplaceSDK.Orders.GetOrderDetails(orderID);
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

  async returnOrder(orderID: string): Promise<MarketplaceOrder> {
    const order = await Orders.Patch('Outgoing', orderID, {
      xp: {
        OrderReturnInfo: {
          HasReturn: true,
          Resolved: false,
        },
      },
    });
    return order as MarketplaceOrder;
  }

  async returnLineItem(
    orderID: string,
    lineItemID: string,
    quantityToReturn: number,
    returnReason: string
  ): Promise<MarketplaceLineItem> {
    const patch = {
      xp: {
        LineItemReturnInfo: {
          QuantityToReturn: quantityToReturn,
          ReturnReason: returnReason,
          Resolved: false,
        },
      },
    };
    const line = await LineItems.Patch('Outgoing', orderID, lineItemID, patch);
    MarketplaceSDK.Orders.RequestReturnEmail(orderID); // Not awaited. Can return before email succeeds.
    return line;
  }
}
