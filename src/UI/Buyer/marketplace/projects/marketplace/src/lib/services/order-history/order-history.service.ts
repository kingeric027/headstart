import { Injectable } from '@angular/core';
import {
  OcOrderService,
  ListPromotion,
  ListPayment,
  OrderApproval,
  OcLineItemService,
  OcMeService,
  LineItem,
  OcSupplierService,
  OcSupplierAddressService,
  Supplier,
  Address,
  OcTokenService,
} from '@ordercloud/angular-sdk';
import { uniqBy as _uniqBy } from 'lodash';
import { ReorderHelperService } from '../reorder/reorder.service';
import { PaymentHelperService } from '../payment-helper/payment-helper.service';
import {
  OrderReorderResponse,
  OrderDetails,
  ShipmentWithItems,
  ShipmentItemWithLineItem,
  MarketplaceOrder,
  LineItemGroupSupplier,
  AppConfig,
} from '../../shopper-context';
import { OrderFilterService, IOrderFilters } from './order-filter.service';
import { MarketplaceAddressBuyer, MarketplaceUserGroup } from 'marketplace-javascript-sdk';
import { HttpHeaders, HttpClient } from '@angular/common/http';

export interface IOrderHistory {
  activeOrderID: string;
  filters: IOrderFilters;
  approveOrder(orderID?: string, Comments?: string, AllowResubmit?: boolean): Promise<MarketplaceOrder>;
  declineOrder(orderID?: string, Comments?: string, AllowResubmit?: boolean): Promise<MarketplaceOrder>;
  validateReorder(orderID: string, lineItems: LineItem[]): Promise<OrderReorderResponse>;
  getOrderDetails(orderID?: string): Promise<OrderDetails>;
  getLineItemSuppliers(liGroups: LineItem[][]): Promise<LineItemGroupSupplier[]>;
  listShipments(orderID?: string): Promise<ShipmentWithItems[]>;
}

@Injectable({
  providedIn: 'root',
})
export class OrderHistoryService implements IOrderHistory {
  activeOrderID: string; // TODO - make this read-only in components

  constructor(
    public filters: OrderFilterService,
    private ocOrderService: OcOrderService,
    private ocMeService: OcMeService,
    private paymentHelper: PaymentHelperService,
    private reorderHelper: ReorderHelperService,
    private ocLineItemService: OcLineItemService,
    private ocSupplierService: OcSupplierService,
    private ocSupplierAddressService: OcSupplierAddressService,

    // remove below when sdk is regenerated
    private ocTokenService: OcTokenService,
    private httpClient: HttpClient,
    private appConfig: AppConfig
  ) {}

  async getLocationsUserCanView(): Promise<MarketplaceAddressBuyer[]> {
    // add strong type when sdk is regenerated
    const accessUserGroups = await this.ocMeService
      .ListUserGroups({ filters: { 'xp.Type': 'OrderAccess' } })
      .toPromise();
    const locationRequests = accessUserGroups.Items.map(a => this.ocMeService.GetAddress(a.xp.Location).toPromise());
    const locationResponses = await Promise.all(locationRequests);
    return locationResponses;
  }

  async approveOrder(
    orderID: string = this.activeOrderID,
    Comments = '',
    AllowResubmit = false
  ): Promise<MarketplaceOrder> {
    return await this.ocOrderService.Approve('outgoing', orderID, { Comments, AllowResubmit }).toPromise();
  }

  async declineOrder(
    orderID: string = this.activeOrderID,
    Comments = '',
    AllowResubmit = true
  ): Promise<MarketplaceOrder> {
    return await this.ocOrderService.Decline('outgoing', orderID, { Comments, AllowResubmit }).toPromise();
  }

  async validateReorder(orderID: string = this.activeOrderID, lineItems: LineItem[]): Promise<OrderReorderResponse> {
    return this.reorderHelper.validateReorder(orderID, lineItems);
  }

  async getOrderDetails(orderID: string = this.activeOrderID): Promise<OrderDetails> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.ocTokenService.GetAccess()}`,
    });
    const url = `${this.appConfig.middlewareUrl}/order/${orderID}/details`;
    return this.httpClient
      .get<OrderDetails>(url, { headers: headers })
      .toPromise();
  }

  async getLineItemSuppliers(liGroups: LineItem[][]): Promise<LineItemGroupSupplier[]> {
    const suppliers: LineItemGroupSupplier[] = [];
    for (const group of liGroups) {
      const line = group[0];
      if (line?.SupplierID) {
        const supplier = await this.ocSupplierService.Get(line.SupplierID).toPromise();
        const shipFrom = await this.ocSupplierAddressService.Get(line.SupplierID, line.ShipFromAddressID).toPromise();
        suppliers.push({ supplier, shipFrom });
      }
    }
    return suppliers;
  }

  async listShipments(orderID: string = this.activeOrderID): Promise<ShipmentWithItems[]> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.ocTokenService.GetAccess()}`,
    });
    const url = `${this.appConfig.middlewareUrl}/order/${orderID}/shipmentswithitems`;
    return this.httpClient
      .get<ShipmentWithItems[]>(url, { headers: headers })
      .toPromise();
  }
}
