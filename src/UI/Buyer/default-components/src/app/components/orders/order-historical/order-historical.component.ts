import { Component, Input } from '@angular/core';
import { OrderDetails, MarketplaceOrder } from 'marketplace';

import { OrderApproval, ListLineItem, Promotion, ListPayment } from '@ordercloud/angular-sdk';

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

    @Input() set orderDetails(value: OrderDetails) {
        this.order = value.order;
        this.lineItems = value.lineItems;
        this.promotions = value.promotions.Items;
        this.payments = value.payments;
        this.approvals = value.approvals;
    }
}

