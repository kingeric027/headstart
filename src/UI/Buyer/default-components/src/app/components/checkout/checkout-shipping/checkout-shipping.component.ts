import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { LineItem, MarketplaceOrder, ProposedShipment, ShipmentPreference, ListLineItem } from 'marketplace';

@Component({
  templateUrl: './checkout-shipping.component.html',
  styleUrls: ['./checkout-shipping.component.scss']
})
export class OCMCheckoutShipping implements OnInit {
  _proposedShipments = null;
  _lineItemsByProposedShipment = null;

  @Input() set proposedShipments(value: ProposedShipment[]) {
    this._proposedShipments = value;
    this._lineItemsByProposedShipment = value.map(proposedShipment => {
      return this.getLineItemsForProposedShipment(proposedShipment);
    });
  }
  @Input() order: MarketplaceOrder;
  @Input() lineItems: ListLineItem;
  @Output() selectShipRate = new EventEmitter<ShipmentPreference>();
  @Output() continue = new EventEmitter();

  constructor() {}

  ngOnInit(): void {}

  getLineItemsForProposedShipment(proposedShipment: ProposedShipment): LineItem[] {
    return proposedShipment.ProposedShipmentItems.map(proposedShipmentItem => {
      return this.lineItems.Items.find(li => li.ID === proposedShipmentItem.LineItemID);
    });
  }

  getSupplierID(proposedShipment: ProposedShipment): string {
    if (!this.order.xp) return null;
    const line = this.getFirstLineItem(proposedShipment)
    return line.SupplierID;
  }

  getShipFromAddressID(proposedShipment: ProposedShipment): string {
    if (!this.order.xp) return null;
    const line = this.getFirstLineItem(proposedShipment)
    return line.ShipFromAddressID;
  }

  getFirstLineItem(proposedShipment: ProposedShipment): LineItem {
    const firstLineItemID = proposedShipment.ProposedShipmentItems[0].LineItemID;
    return this.lineItems.Items.find(lineItem => lineItem.ID === firstLineItemID);
  }

  selectRate(selection: ShipmentPreference): void {
    this.selectShipRate.emit(selection);
  }

  onContinueClicked(): void {
    this.continue.emit();
  }
}
