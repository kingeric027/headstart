import { CurrencyPipe } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MarketplaceOrder, LineItem } from 'marketplace';

@Component({
  templateUrl: './order-summary.component.html',
  styleUrls: ['./order-summary.component.scss'],
})
export class OCMOrderSummary {
  shippingSelected: boolean;
  subtotal: number;
  total: number;
  poTotal: number;

  overrideShippingAndTaxText = '';
  
  poLineItemCount: number;
  standardLineItemCount: number;

  _order: MarketplaceOrder;
  _lineItems: LineItem[];
  
  @Input() checkoutPanel = '';
  @Input() set order(value: MarketplaceOrder) {
    this._order = value;
    this.setDisplayValues();
  }
  @Input() set lineItems(value: LineItem[]) {
    this._lineItems = value;
    this.setDisplayValues();
  }
  
  setDisplayValues(): void {
    if(this._lineItems?.length && this._order){
      this.getOverrideText();

      const standardLineItems = this._lineItems.filter(li => !(li.Product.xp?.ProductType ===  'PurchaseOrder'));
      const poLineItems = this._lineItems.filter(li => li.Product.xp?.ProductType ===  'PurchaseOrder');
      this.standardLineItemCount = standardLineItems.length;
      this.poLineItemCount = poLineItems.length;
      
      // subtotal only includes standardlineItems
      this.subtotal = standardLineItems.reduce((accumulator, li) => (li.Quantity * li.UnitPrice) + accumulator, 0);
      this.total = this.displayTotal();
      this.poTotal = poLineItems.reduce((accumulator, li) => (li.Quantity * li.UnitPrice) + accumulator, 0);
    }
  }

  getOverrideText(): void {
      /* if there is override text for shipping and tax 
       * we show that and calculate the order total differently */
    if (this.checkoutPanel === 'cart') {
      this.overrideShippingAndTaxText = 'Calculated during checkout';
    }
    if (
      (this.checkoutPanel === 'shippingAddress' && !this.shippingSelected) ||
      (this.checkoutPanel === 'shippingSelection' && !this.shippingSelected)
      ) {
        this.overrideShippingAndTaxText = 'Pending selections';
    }
  }

  displayTotal(): number {
    if(this.overrideShippingAndTaxText) {
      return this.subtotal;
    } else {
      return this._order.ShippingCost + this._order.TaxCost + this.subtotal;
    }
  }
}
