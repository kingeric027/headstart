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
} from '../../shopper-context';
import { OrderFilterService, IOrderFilters } from './order-filter.service';

export interface IOrderHistory {
  activeOrderID: string;
  filters: IOrderFilters;
  approveOrder(orderID?: string, Comments?: string, AllowResubmit?: boolean): Promise<MarketplaceOrder>;
  declineOrder(orderID?: string, Comments?: string, AllowResubmit?: boolean): Promise<MarketplaceOrder>;
  validateReorder(orderID?: string): Promise<OrderReorderResponse>;
  getOrderDetails(isOrderToApprove: boolean, orderID?: string): Promise<OrderDetails>;
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
    private ocSupplierAddressService: OcSupplierAddressService
  ) {}

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

  async validateReorder(orderID: string = this.activeOrderID): Promise<OrderReorderResponse> {
    return this.reorderHelper.validateReorder(orderID);
  }

  async getOrderDetails(isOrderToApprove: boolean, orderID: string = this.activeOrderID): Promise<OrderDetails> {
    const res = await Promise.all([
      this.ocOrderService.Get('outgoing', orderID).toPromise(),
      this.ocLineItemService.List('outgoing', orderID).toPromise(),
      this.getPromotions(orderID),
      this.getPayments(isOrderToApprove, orderID),
      this.getApprovals(orderID),
    ]);
    return { order: res[0], lineItems: res[1], promotions: res[2], payments: res[3], approvals: res[4] };
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
    const [lineItems, shipments] = await Promise.all([
      this.ocLineItemService.List('outgoing', orderID).toPromise(),
      this.ocMeService.ListShipments({ orderID }).toPromise(),
    ]);
    const getShipmentItems = shipments.Items.map(shipment =>
      this.ocMeService.ListShipmentItems(shipment.ID).toPromise()
    );
    const shipmentItems = (await Promise.all(getShipmentItems)).map(si => si.Items) as ShipmentItemWithLineItem[][];
    shipments.Items.map((shipment: ShipmentWithItems, index) => {
      shipment.ShipmentItems = shipmentItems[index];
      shipment.ShipmentItems.map((shipmentItem: ShipmentItemWithLineItem) => {
        shipmentItem.LineItem = lineItems.Items.find(li => li.ID === shipmentItem.LineItemID);
        return shipmentItem;
      });
      return shipment;
    });
    return shipments.Items as ShipmentWithItems[];
  }

  private async getPromotions(orderID: string): Promise<ListPromotion> {
    return this.ocOrderService.ListPromotions('outgoing', orderID).toPromise();
  }

  private async getPayments(isOrderToApprove: boolean, orderID: string): Promise<ListPayment> {
    if (isOrderToApprove) {
      // approver likely does not have the access required to get this information
      return null;
    } else {
      return this.paymentHelper.ListPaymentsOnOrder(orderID, false);
    }
  }

  private async getApprovals(orderID: string): Promise<OrderApproval[]> {
    const approvals = await this.ocOrderService.ListApprovals('outgoing', orderID).toPromise();
    approvals.Items = approvals.Items.filter(x => x.Approver);
    return _uniqBy(approvals.Items, x => x.Comments);
  }
}
