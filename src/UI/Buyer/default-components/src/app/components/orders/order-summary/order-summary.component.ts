import { CurrencyPipe } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MarketplaceOrder, LineItem } from 'marketplace';
import { OrderSummaryMeta } from 'src/app/services/purchase-order.helper';

@Component({
  templateUrl: './order-summary.component.html',
  styleUrls: ['./order-summary.component.scss'],
})
export class OCMOrderSummary {
  @Input() orderSummaryMeta: OrderSummaryMeta;
}
