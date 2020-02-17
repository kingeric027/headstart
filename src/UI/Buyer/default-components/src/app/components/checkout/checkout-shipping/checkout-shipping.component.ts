import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { LineItem, MarketplaceOrder, ProposedShipment, ProposedShipmentSelection, ListLineItem, ListProposedShipment } from 'marketplace';

@Component({
  templateUrl: './checkout-shipping.component.html',
  styleUrls: ['./checkout-shipping.component.scss']
})
export class OCMCheckoutShipping implements OnInit {
  _proposedShipments = null;
  _lineItemsByProposedShipment = null;

  @Input() set proposedShipments(value: ListProposedShipment) {
    this._proposedShipments = value;
    this._lineItemsByProposedShipment = value.Items.map(proposedShipment => {
      return this.getLineItemsForProposedShipment(proposedShipment);
    });
  }
  @Input() order: MarketplaceOrder;
  @Input() lineItems: ListLineItem;
  @Output() selectShipRate = new EventEmitter<ProposedShipmentSelection>();
  @Output() continue = new EventEmitter();

  constructor() {}

  ngOnInit(): void {}

  getLineItemsForProposedShipment(proposedShipment: ProposedShipment): LineItem[] {
    return proposedShipment.ProposedShipmentItems.map(proposedShipmentItem => {
      return this.lineItems.Items.find(li => li.ID === proposedShipmentItem.LineItemID);
    });
  }

  getExistingSelectionID(proposedShipment: ProposedShipment): string {
    // ultimately shipment selections will be on the order object or lineItem object and will likely
    // be organized by proposedShipmentID?, for not we are identifying shipment selections on xp
    // based on the shipfromaddressID
    if (!this.order.xp) return null;
    const line = this.getFirstLineItem(proposedShipment)
    const supplierID = line.SupplierID
    const shipFromAddressID = line.ShipFromAddressID
    const proposedShipmentSelection = this.order.xp.ProposedShipmentSelections
      .find(selection => selection.ShipFromAddressID === shipFromAddressID && selection.SupplierID === supplierID);
    return proposedShipmentSelection?.ProposedShipmentOptionID;
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

  selectRate(selection: ProposedShipmentSelection): void {
    this.selectShipRate.emit(selection);
  }

  onContinueClicked(): void {
    this.continue.emit();
  }
}
