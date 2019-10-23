import { Component, Input } from '@angular/core';
import { OCMComponent } from '../base-component';
import { OrderDetails } from 'marketplace';

import { Order, OrderApproval, LineItem, Promotion, ListPayment } from '@ordercloud/angular-sdk';

@Component({
  templateUrl: './order-historical.component.html',
  styleUrls: ['./order-historical.component.scss'],
})
export class OCMOrderHistorical extends OCMComponent {
    order: Order;
    lineItems: LineItem[] = [];
    promotions: Promotion[] = [];
    payments: ListPayment;
    approvals: OrderApproval[] = [];

    @Input() set orderDetails(value: OrderDetails) {
        this.order = value.order;
        this.lineItems = value.lineItems.Items;
        this.promotions = value.promotions.Items;
        this.payments = value.payments;
        this.approvals = value.approvals;
    }

    ngOnContextSet() {}
}

