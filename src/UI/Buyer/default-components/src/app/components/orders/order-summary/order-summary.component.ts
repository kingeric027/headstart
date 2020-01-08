import { Component, Input } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { MarketplaceOrder } from 'marketplace';

@Component({
  templateUrl: './order-summary.component.html',
  styleUrls: ['./order-summary.component.scss'],
})
export class OCMOrderSummary {
  @Input() order: MarketplaceOrder;

  displayTax() {
    if (!this.order.xp || !this.order.xp.AvalaraTaxTransactionCode) {
      return 'Calculated during checkout';
    }
    return new CurrencyPipe('en-US').transform(this.order.ShippingCost);
  }

  displayShipping() {
    if (!this.order.xp || this.order.xp.ShippingSelections.length === 0) {
      return 'Calculated during checkout';
    }
    return new CurrencyPipe('en-US').transform(this.order.TaxCost);
  }
}
