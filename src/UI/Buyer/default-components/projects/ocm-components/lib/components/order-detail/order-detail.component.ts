import { Component } from '@angular/core';
import { Order, OrderApproval, LineItem, Promotion, ListPayment } from '@ordercloud/angular-sdk';
import { OrderDetails } from 'shopper-context-interface';
import { OCMComponent } from '../base-component';

@Component({
  templateUrl: './order-detail.component.html',
  styleUrls: ['./order-detail.component.scss'],
})
export class OCMOrderDetails extends OCMComponent {
  order: Order;
  lineItems: LineItem[] = [];
  promotions: Promotion[] = [];
  payments: ListPayment;
  approvals: OrderApproval[] = [];
  detailedOrder: OrderDetails;

  async ngOnContextSet() {
    this.detailedOrder = await this.context.orderHistory.getOrderDetails();
    this.order = this.detailedOrder.order;
    this.lineItems = this.detailedOrder.lineItems.Items;
    this.promotions = this.detailedOrder.promotions.Items;
    this.payments = this.detailedOrder.payments;
    this.approvals = this.detailedOrder.approvals;
  }
}
