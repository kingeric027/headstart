import { CurrencyPipe } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MarketplaceOrder } from 'marketplace';

@Component({
  templateUrl: './order-summary.component.html',
  styleUrls: ['./order-summary.component.scss'],
})
export class OCMOrderSummary {
  @Input() order: MarketplaceOrder;
  @Input() checkoutPanel = '';

  displayTax() {
    if (this.checkoutPanel === 'cart') {
      return 'Calculated during checkout';
    }
    if (this.checkoutPanel === 'shippingAddress' || this.checkoutPanel === 'shippingSelection') {
      return 'Pending selections';
    }
    return new CurrencyPipe('en-US').transform(this.order.TaxCost);
  }

  displayShipping() {
    if (this.checkoutPanel === 'cart') {
      return 'Calculated during checkout';
    }
    if (this.checkoutPanel === 'shippingAddress' || this.checkoutPanel === 'shippingSelection') {
      return 'Pending selections';
    }
    return new CurrencyPipe('en-US').transform(this.order.ShippingCost);
  }
}
