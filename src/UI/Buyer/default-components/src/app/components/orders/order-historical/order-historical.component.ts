import { Component, Input } from '@angular/core';
import { OrderDetails, MarketplaceOrder, ShopperContextService, OrderType } from 'marketplace';
import { OrderApproval, ListLineItem, Promotion, ListPayment, BuyerAddress } from '@ordercloud/angular-sdk';
import { isQuoteOrder } from '../../../services/orderType.helper';

@Component({
  templateUrl: './order-historical.component.html',
  styleUrls: ['./order-historical.component.scss'],
})
export class OCMOrderHistorical {
  order: MarketplaceOrder;
  lineItems: ListLineItem;
  promotions: Promotion[] = [];
  payments: ListPayment;
  approvals: OrderApproval[] = [];
  isQuoteOrder = isQuoteOrder;
  buyerLocation: BuyerAddress;
  @Input() set orderDetails(value: OrderDetails) {
    this.order = value.order;
    this.lineItems = value.lineItems;
    this.promotions = value.promotions.Items;
    this.payments = value.payments;
    this.approvals = value.approvals;
    this.getBuyerLocation();
  }

  constructor(private context: ShopperContextService) { }

  async getBuyerLocation(): Promise<void> {
    const buyerLocation = await this.context.currentUser.addresses.listBuyerLocations({filters: {ID: this.order.xp.BuyerLocationID} });
    this.buyerLocation = buyerLocation.Items[0];
  }
}
