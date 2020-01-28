import { Component, Input, OnChanges } from '@angular/core';
import { OrderDetails, MarketplaceOrder, ShopperContextService, LineItem } from 'marketplace';
import { groupBy as _groupBy } from 'lodash';
import { OrderApproval, ListLineItem, Promotion, ListPayment } from '@ordercloud/angular-sdk';

@Component({
  templateUrl: './order-historical.component.html',
  styleUrls: ['./order-historical.component.scss'],
})
export class OCMOrderHistorical implements OnChanges {
  order: MarketplaceOrder;
  lineItems: ListLineItem;
  promotions: Promotion[] = [];
  payments: ListPayment;
  approvals: OrderApproval[] = [];
  liGroups: any;
  liGroupedByShipFrom: LineItem[][];

  @Input() set orderDetails(value: OrderDetails) {
    this.order = value.order;
    this.lineItems = value.lineItems;
    this.promotions = value.promotions.Items;
    this.payments = value.payments;
    this.approvals = value.approvals;
  }

  async ngOnChanges() {
    this.liGroups = _groupBy(this.lineItems.Items, li => li.ShipFromAddressID);
    this.liGroupedByShipFrom = Object.values(this.liGroups);
  }
}
