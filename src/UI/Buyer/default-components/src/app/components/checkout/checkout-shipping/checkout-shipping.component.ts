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

  // TODO - this will change. also it is repeated
  getShipFromAddressID(lineItems: LineItem[]): string {
    if (!lineItems || lineItems.length === 0) return null;
    return lineItems[0].ShipFromAddressID; 
  }

  getshippingOptions(lineItems: LineItem[]): ShippingOptions {
    const shipFromAddressID = this.getShipFromAddressID(lineItems);
    if (shipFromAddressID === null) return null; 
    return this.shippingOptions.find(o => o.ShipFromAddressID === shipFromAddressID);
  }

  selectRate(selection: ShippingSelection) {
    this.context.currentOrder.selectShippingRate(selection);
  }

  getExistingSelection(lineItems: LineItem[]): ShippingSelection {
    const shipFromAddressID = this.getShipFromAddressID(lineItems);
    if (shipFromAddressID === null) return null; 
    return this.order.xp.ShippingSelections.find(s => s.ShipFromAddressID === shipFromAddressID);
  }

}
