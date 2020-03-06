import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { LineItem, MarketplaceOrder, ShipmentEstimate, ShipmentPreference, ListLineItem } from 'marketplace';

@Component({
  templateUrl: './checkout-shipping.component.html',
  styleUrls: ['./checkout-shipping.component.scss'],
})
export class OCMCheckoutShipping implements OnInit {
  _shipmentEstimates = null;
  _lineItemsByShipmentEstimate = null;

  @Input() set shipmentEstimates(value: ShipmentEstimate[]) {
    this._shipmentEstimates = value;
    this._lineItemsByShipmentEstimate = value.map(shipmentEstimate => {
      return this.getLineItemsForShipmentEstimate(shipmentEstimate);
    });
  }
  @Input() order: MarketplaceOrder;
  @Input() lineItems: ListLineItem;
  @Output() selectShipRate = new EventEmitter<ShipmentPreference>();
  @Output() continue = new EventEmitter();

  constructor() {}

  ngOnInit(): void {}

  getLineItemsForShipmentEstimate(shipmentEstimate: ShipmentEstimate): LineItem[] {
    return shipmentEstimate.ShipmentEstimateItems.map(shipmentEstimateItem => {
      return this.lineItems.Items.find(li => li.ID === shipmentEstimateItem.LineItemID);
    });
  }

  getSupplierID(shipmentEstimate: ShipmentEstimate): string {
    if (!this.order.xp) return null;
    const line = this.getFirstLineItem(shipmentEstimate);
    return line.SupplierID;
  }

  getShipFromAddressID(shipmentEstimate: ShipmentEstimate): string {
    if (!this.order.xp) return null;
    const line = this.getFirstLineItem(shipmentEstimate);
    return line.ShipFromAddressID;
  }

  getFirstLineItem(shipmentEstimate: ShipmentEstimate): LineItem {
    const firstLineItemID = shipmentEstimate.ShipmentEstimateItems[0].LineItemID;
    return this.lineItems.Items.find(lineItem => lineItem.ID === firstLineItemID);
  }

  selectRate(selection: ShipmentPreference): void {
    this.selectShipRate.emit(selection);
  }

  onContinueClicked(): void {
    this.continue.emit();
  }
}
