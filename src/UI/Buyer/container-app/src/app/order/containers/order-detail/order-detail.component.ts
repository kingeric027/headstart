import { Component, OnInit, Input } from '@angular/core';
import { Order, OrderApproval, LineItem, Promotion, ListPayment } from '@ordercloud/angular-sdk';
import { OrderDetails, IShopperContext } from 'shopper-context-interface';

@Component({
  selector: 'ocm-order-details',
  templateUrl: './order-detail.component.html',
  styleUrls: ['./order-detail.component.scss'],
})
export class OrderDetailsComponent implements OnInit {
  @Input() orderID: string;
  @Input() context: IShopperContext;

  order: Order;
  lineItems: LineItem[] = [];
  promotions: Promotion[] = [];
  payments: ListPayment;
  approvals: OrderApproval[] = [];
  detailedOrder: OrderDetails;

  async ngOnInit() {
    this.detailedOrder = await this.context.orderHistory.getOrderDetails(this.orderID);
    this.order = this.detailedOrder.order;
    this.lineItems = this.detailedOrder.lineItems.Items;
    this.promotions = this.detailedOrder.promotions.Items;
    this.payments = this.detailedOrder.payments;
    this.approvals = this.detailedOrder.approvals;
  }
}
