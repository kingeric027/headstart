import { CurrencyPipe } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MarketplaceOrder } from 'marketplace';

@Component({
  templateUrl: './order-summary.component.html',
  styleUrls: ['./order-summary.component.scss'],
})
export class OCMOrderSummary {
  shippingSelected: boolean;
  @Input() order: MarketplaceOrder;
  @Input() checkoutPanel = '';

  displayTax(): string {
    if (this.checkoutPanel === 'cart') {
      return 'Calculated during checkout';
    }
    if (this.checkoutPanel === 'shippingAddress' && !this.shippingSelected || this.checkoutPanel === 'shippingSelection' && !this.shippingSelected) {
      return 'Pending selections';
    }
    return new CurrencyPipe('en-US').transform(this.order.TaxCost);
  }

  displayShipping(): string {
    if (this.checkoutPanel === 'cart') {
      return 'Calculated during checkout';
    }
    if (this.checkoutPanel === 'shippingAddress' && !this.shippingSelected || this.checkoutPanel === 'shippingSelection' && !this.shippingSelected) {
      return 'Pending selections';
    }
    this.shippingSelected = true;
    return new CurrencyPipe('en-US').transform(this.order.ShippingCost);
  }

  displayTotal(): string {
    if (this.checkoutPanel === 'cart' ||
        this.checkoutPanel === 'shippingAddress' && !this.shippingSelected ||
        this.checkoutPanel === 'shippingSelection' && !this.shippingSelected) {
      return new CurrencyPipe('en-US').transform(this.order.Subtotal);
    }
    return new CurrencyPipe('en-US').transform(this.order.Total);
  }
}
