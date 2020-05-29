import { Component, Input } from '@angular/core';
import { OrderDetails, MarketplaceOrder, ShopperContextService } from 'marketplace';
import {
  OrderApproval,
  ListLineItem,
  Promotion,
  ListPayment,
  BuyerAddress,
  Payment,
  LineItem,
} from '@ordercloud/angular-sdk';
import { isQuoteOrder } from '../../../services/orderType.helper';

@Component({
  templateUrl: './order-historical.component.html',
  styleUrls: ['./order-historical.component.scss'],
})
export class OCMOrderHistorical {
  order: MarketplaceOrder;
  lineItems: LineItem[] = [];
  promotions: Promotion[] = [];
  payments: Payment[] = [];
  approvals: OrderApproval[] = [];
  isQuoteOrder = isQuoteOrder;
  buyerLocation: BuyerAddress;
  @Input() isOrderToApprove = false;
  @Input() set orderDetails(value: OrderDetails) {
    this.order = value.Order;
    this.lineItems = value.LineItems;
    this.promotions = value.Promotions;
    this.payments = value.Payments;
    this.approvals = value.Approvals.filter(a => a.Approver);
    this.getBuyerLocation(this.order.BillingAddressID);
  }

  constructor(private context: ShopperContextService) {}

  async getBuyerLocation(addressID): Promise<void> {
    if (!this.isQuoteOrder(this.order)) {
      const buyerLocation = await this.context.addresses.get(addressID);
      this.buyerLocation = buyerLocation;
    } else this.buyerLocation = null;
  }
}
