import { Injectable } from '@angular/core';
import { OcOrderService, Order, ListPromotion, ListPayment, OrderApproval, OcLineItemService, ListShipment, OcMeService, ShipmentItem } from '@ordercloud/angular-sdk';
import { uniqBy as _uniqBy } from 'lodash';
import { ReorderHelperService } from '../reorder/reorder.service';
import { PaymentHelperService } from '../payment-helper/payment-helper.service';
import { IOrderHistory, OrderReorderResponse, OrderDetails, MyShipment, MyShipmentItem } from '../../shopper-context';

@Injectable({
  providedIn: 'root',
})
export class OrderHistoryService implements IOrderHistory {
  activeOrderID: string; // TODO - make this read-only in components
  activeOrderDetails: OrderDetails;

  constructor(
    private ocOrderService: OcOrderService,
    private ocMeService: OcMeService,
    private paymentHelper: PaymentHelperService,
    private reorderHelper: ReorderHelperService,
    private ocLineItemService: OcLineItemService
  ) {}

  async approveOrder(orderID: string = this.activeOrderID, Comments: string = '', AllowResubmit: boolean = false): Promise<Order> {
    return await this.ocOrderService.Approve('outgoing', orderID, { Comments, AllowResubmit }).toPromise();
  }

  async declineOrder(orderID: string = this.activeOrderID, Comments: string = '', AllowResubmit: boolean = false): Promise<Order> {
    return await this.ocOrderService.Decline('outgoing', orderID, { Comments, AllowResubmit }).toPromise();
  }

  async validateReorder(orderID: string = this.activeOrderID): Promise<OrderReorderResponse> {
    return this.reorderHelper.validateReorder(orderID);
  }

  async getOrderDetails(orderID: string = this.activeOrderID): Promise<OrderDetails> {
    const res = await Promise.all([
      this.ocOrderService.Get('outgoing', orderID).toPromise(),
      this.ocLineItemService.List('outgoing', orderID).toPromise(),
      this.getPromotions(orderID),
      this.getPayments(orderID),
      this.getApprovals(orderID),
    ]);
    return { order: res[0], lineItems: res[1], promotions: res[2], payments: res[3], approvals: res[4] };
  }

  private async getPromotions(orderID: string): Promise<ListPromotion> {
    return this.ocOrderService.ListPromotions('outgoing', orderID).toPromise();
  }

  private async getPayments(orderID: string): Promise<ListPayment> {
    return this.paymentHelper.ListPaymentsOnOrder(orderID);
  }

  private async getApprovals(orderID: string): Promise<OrderApproval[]> {
    const approvals = await this.ocOrderService.ListApprovals('outgoing', orderID).toPromise();
    approvals.Items = approvals.Items.filter((x) => x.Approver);
    return _uniqBy(approvals.Items, (x) => x.Comments);
  }

  async listShipments(orderID: string = this.activeOrderID): Promise<MyShipment[]> {
    const [lineItems, shipments] = await Promise.all([
      this.ocLineItemService.List('outgoing', orderID).toPromise(),
      this.ocMeService.ListShipments({ orderID }).toPromise()
    ]);
    const getShipmentItems = shipments.Items.map(shipment => this.ocMeService.ListShipmentItems(shipment.ID).toPromise());
    const shipmentItems = (await Promise.all(getShipmentItems)).map(si => si.Items) as MyShipmentItem[][];
    shipments.Items.map((shipment: MyShipment, index) => {
      shipment.ShipmentItems = shipmentItems[index];
      shipment.ShipmentItems.map((shipmentItem: MyShipmentItem) => {
        shipmentItem.LineItem = lineItems.Items.find(li => li.ID === shipmentItem.LineItemID);
        return shipmentItem;
      });
      return shipment;
    });
    return shipments.Items as MyShipment[];
  }
}
