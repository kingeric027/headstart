import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { MarketplaceOrder, MarketplaceLineItem } from 'marketplace-javascript-sdk'
import { ShipEstimate, ListPage, ShipMethodSelection } from 'ordercloud-javascript-sdk';

@Component({
  templateUrl: './checkout-shipping.component.html',
  styleUrls: ['./checkout-shipping.component.scss'],
})
export class OCMCheckoutShipping implements OnInit {
  _shipEstimates = null;
  _areAllShippingSelectionsMade = false;
  _lineItemsByShipEstimate = null;

  @Input() set shipEstimates(value: ShipEstimate[]) {
    this._shipEstimates = value;
    this._areAllShippingSelectionsMade = this.areAllShippingSelectionsMade(value);
    this._lineItemsByShipEstimate = value.map(shipEstimate => {
      return this.getLineItemsForShipEstimate(shipEstimate);
    });
  }
  @Input() order: MarketplaceOrder;
  @Input() lineItems: ListPage<MarketplaceLineItem>;
  @Output() selectShipRate = new EventEmitter<ShipMethodSelection>();
  @Output() continue = new EventEmitter();

  constructor() {}

  ngOnInit(): void {}

  getLineItemsForShipEstimate(shipEstimate: ShipEstimate): MarketplaceLineItem[] {
    return shipEstimate.ShipEstimateItems.map(shipEstimateItem => {
      return this.lineItems.Items.find(li => li.ID === shipEstimateItem.LineItemID);
    });
  }

  areAllShippingSelectionsMade(shipEstimates: ShipEstimate[]): boolean {
    return shipEstimates.every(shipEstimate => shipEstimate.SelectedShipMethodID)
  }

  getSupplierID(shipEstimate: ShipEstimate): string {
    if (!this.order.xp) return null;
    const line = this.getFirstLineItem(shipEstimate);
    return line?.SupplierID;
  }

  getShipFromAddressID(shipEstimate: ShipEstimate): string {
    if (!this.order.xp) return null;
    const line = this.getFirstLineItem(shipEstimate);
    return line.ShipFromAddressID;
  }

  getFirstLineItem(shipEstimate: ShipEstimate): MarketplaceLineItem {
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
