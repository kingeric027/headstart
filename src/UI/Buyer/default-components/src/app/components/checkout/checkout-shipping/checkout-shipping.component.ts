import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { groupBy as _groupBy } from 'lodash';
import { ShopperContextService, LineItem, ShippingRate, ShippingOptions, ShippingSelection, MarketplaceOrder } from 'marketplace';

@Component({
  templateUrl: './checkout-shipping.component.html',
  styleUrls: ['./checkout-shipping.component.scss']
})
export class OCMCheckoutShipping implements OnInit {
  @Output() continue = new EventEmitter();

  order: MarketplaceOrder;
  liGroups: any;
  liGroupedByShipFrom: LineItem[][];
  shippingOptions: ShippingOptions[] = null;

  constructor(private context: ShopperContextService) {}

  async ngOnInit() {
    this.order = this.context.currentOrder.get();
    const lineItems = this.context.currentOrder.getLineItems();
    this.liGroups = _groupBy(lineItems.Items, li => li.ShipFromAddressID);
    this.liGroupedByShipFrom = Object.values(this.liGroups);
    this.shippingOptions = await this.context.currentOrder.getShippingRates();
  }

  async onContinueClicked() {
    await this.context.currentOrder.calculateTax();
    this.continue.emit();
  }

  getshippingOptions(lineItems: LineItem[]): ShippingOptions {
    const ID = lineItems[0].ShipFromAddressID;
    return this.shippingOptions.find(o => o.ShipFromAddressID === ID);
  }

  getExistingSelection(lineItems: LineItem[]): ShippingSelection {
    const ID = lineItems[0].ShipFromAddressID;
    if (!this.order.xp || this.order.xp.ShippingSelections) return null;
    return this.order.xp.ShippingSelections.find(s => s.ShipFromAddressID === ID);
  }

  selectRate(selection: ShippingSelection) {
    this.context.currentOrder.selectShippingRate(selection);
  }
}
