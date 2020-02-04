import { CurrencyPipe } from '@angular/common';
import { Component, Input, OnChanges } from '@angular/core';
import { MarketplaceOrder } from 'marketplace';

@Component({
  templateUrl: './order-summary.component.html',
  styleUrls: ['./order-summary.component.scss'],
})
export class OCMOrderSummary implements OnChanges {
  shippingSelected: boolean = false;
  orderLocalVar: MarketplaceOrder;
  @Input() order: MarketplaceOrder;
  @Input() checkoutPanel = '';

  ngOnChanges() {
    if (this.orderLocalVar.ShippingCost !== 0 && this.shippingSelected !== true) {
      this.shippingSelected = true;
    };
  }

  displayTax() {
    if (this.checkoutPanel === 'cart') {
      return 'Calculated during checkout';
    }
    if (this.checkoutPanel === 'shippingAddress' && !this.shippingSelected || this.checkoutPanel === 'shippingSelection' && !this.shippingSelected) {
      return 'Pending selections';
    }
    return new CurrencyPipe('en-US').transform(this.order.TaxCost);
  }

  displayShipping() {
    if (this.checkoutPanel === 'cart') {
      return 'Calculated during checkout';
    }
    if (this.checkoutPanel === 'shippingAddress' && !this.shippingSelected && !this.orderLocalVar) {
      this.orderLocalVar = this.order;
      this.orderLocalVar.ShippingCost = 0;
    }
    if (this.checkoutPanel === 'shippingAddress' && !this.shippingSelected || this.checkoutPanel === 'shippingSelection' && !this.shippingSelected) {
      return 'Pending selections';
    }
    this.orderLocalVar.ShippingCost = this.order.ShippingCost;
    return new CurrencyPipe('en-US').transform(this.order.ShippingCost);
  }

  displayTotal() {
    if (this.checkoutPanel === 'cart') {
      return new CurrencyPipe('en-US').transform(this.order.Subtotal);
    }
    if (this.checkoutPanel === 'shippingAddress' && !this.shippingSelected) {
      return new CurrencyPipe('en-US').transform(this.order.Subtotal);
    }
    if (this.checkoutPanel === 'shippingSelection' && !this.shippingSelected) {
      return new CurrencyPipe('en-US').transform(this.order.Subtotal);
    }
    return new CurrencyPipe('en-US').transform(this.order.Total);
  }
}
