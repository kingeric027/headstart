import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { LineItem, MarketplaceOrder, ShipEstimate, ShipMethodSelection, ListLineItem } from 'marketplace';

@Component({
  templateUrl: './checkout-shipping.component.html',
  styleUrls: ['./checkout-shipping.component.scss'],
})
export class OCMCheckoutShipping implements OnInit {
  _shipEstimates = null;
  _lineItemsByShipEstimate = null;

  @Input() set shipEstimates(value: ShipEstimate[]) {
    this._shipEstimates = value;
    this._lineItemsByShipEstimate = value.map(shipEstimate => {
      return this.getLineItemsForShipEstimate(shipEstimate);
    });
  }
  @Input() order: MarketplaceOrder;
  @Input() lineItems: ListLineItem;
  @Output() selectShipRate = new EventEmitter<ShipMethodSelection>();
  @Output() continue = new EventEmitter();

  constructor() {}

  ngOnInit(): void {}

  getLineItemsForShipEstimate(shipEstimate: ShipEstimate): LineItem[] {
    return shipEstimate.ShipEstimateItems.map(shipEstimateItem => {
      return this.lineItems.Items.find(li => li.ID === shipEstimateItem.LineItemID);
    });
  }

  getSupplierID(shipEstimate: ShipEstimate): string {
    if (!this.order.xp) return null;
    const line = this.getFirstLineItem(shipEstimate);
    return line.SupplierID;
  }

  getShipFromAddressID(shipEstimate: ShipEstimate): string {
    if (!this.order.xp) return null;
    const line = this.getFirstLineItem(shipEstimate);
    return line.ShipFromAddressID;
  }

  getFirstLineItem(shipEstimate: ShipEstimate): LineItem {
    const firstLineItemID = shipEstimate.ShipEstimateItems[0].LineItemID;
    return this.lineItems.Items.find(lineItem => lineItem.ID === firstLineItemID);
  }

  selectRate(selection: ShipMethodSelection): void {
    this.selectShipRate.emit(selection);
  }

  onContinueClicked(): void {
    this.continue.emit();
  }
}
