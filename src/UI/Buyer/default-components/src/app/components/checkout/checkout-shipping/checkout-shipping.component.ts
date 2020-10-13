import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { MarketplaceOrder, MarketplaceLineItem } from '@ordercloud/headstart-sdk'
import { ShipEstimate, ListPage, ShipMethodSelection } from 'ordercloud-javascript-sdk';
import { faExclamationCircle } from '@fortawesome/free-solid-svg-icons';

function notEmpty<TValue>(value: TValue | null | undefined): value is TValue {
  return value !== null && value !== undefined;
}


@Component({
  templateUrl: './checkout-shipping.component.html',
  styleUrls: ['./checkout-shipping.component.scss'],
})
export class OCMCheckoutShipping implements OnInit {
  @Input() order?: MarketplaceOrder;
  @Input() lineItems?: ListPage<MarketplaceLineItem>;
  @Output() selectShipRate = new EventEmitter<ShipMethodSelection>();
  @Output() continue = new EventEmitter();
  @Input() set shipEstimates(value: ShipEstimate[]) {
    this._shipEstimates = value;
    this._areAllShippingSelectionsMade = this.areAllShippingSelectionsMade(value);
    this._lineItemsByShipEstimate = value.map(shipEstimate => {
      return this.getLineItemsForShipEstimate(shipEstimate);
    }).filter(notEmpty)
  }

  _shipEstimates?: ShipEstimate[];
  _areAllShippingSelectionsMade = false;
  _lineItemsByShipEstimate?: MarketplaceLineItem[][];
  faExclamationCircle = faExclamationCircle;

  constructor() {
    this.order = undefined;
    this.lineItems = undefined;
  }

  ngOnInit(): void { }

  getLineItemsForShipEstimate(shipEstimate: ShipEstimate): MarketplaceLineItem[] {
    if (!shipEstimate.ShipEstimateItems) {
      return [];
    }
    return shipEstimate.ShipEstimateItems
      .map(shipEstimateItem => {
        if (!this.lineItems || !this.lineItems.Items) {
          return undefined;
        }
        return this.lineItems.Items.find(li => li.ID === shipEstimateItem.LineItemID);
      })
      .filter(notEmpty);
  }

  areAllShippingSelectionsMade(shipEstimates: ShipEstimate[]): boolean {
    return shipEstimates.every(shipEstimate => shipEstimate.SelectedShipMethodID)
  }

  getSupplierID(shipEstimate: ShipEstimate): string | undefined {
    if (!this.order || !this.order.xp) return;
    const line = this.getFirstLineItem(shipEstimate);
    return line?.SupplierID;
  }

  getShipFromAddressID(shipEstimate: ShipEstimate): string | undefined {
    if (!this.order || !this.order.xp) return;
    const line = this.getFirstLineItem(shipEstimate);
    return line?.ShipFromAddressID;
  }

  getFirstLineItem(shipEstimate: ShipEstimate): MarketplaceLineItem | undefined {
    if (!shipEstimate || !shipEstimate.ShipEstimateItems || !shipEstimate.ShipEstimateItems.length) {
      return;
    }
    if (!this.lineItems || !this.lineItems.Items) {
      return;
    }
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
