import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { groupBy as _groupBy } from 'lodash';
import { ShopperContextService, LineItem, MarketplaceOrder, ProposedShipment, ProposedShipmentSelection, ListLineItem } from 'marketplace';

@Component({
  templateUrl: './checkout-shipping.component.html',
  styleUrls: ['./checkout-shipping.component.scss']
})
export class OCMCheckoutShipping implements OnInit {
  @Input() proposedShipments: ProposedShipment[] = null;
  @Input() order: MarketplaceOrder;
  @Input() lineItems: ListLineItem;
  @Output() selectShipRate = new EventEmitter<ProposedShipmentSelection>();
  @Output() continue = new EventEmitter();

  constructor() {}

  ngOnInit() {}

  getLineItemsForProposedShipment(proposedShipment: ProposedShipment): LineItem[] {
    return proposedShipment.ProposedShipmentItems.map(proposedShipmentItem => {
      return this.lineItems.Items.find(li => li.ID === proposedShipmentItem.LineItemID);
    });
  }

  getExistingSelectionID(proposedShipment: ProposedShipment): string {
    // ultimately shipment selections will be on the order object or lineItem object and will likely
    // be organized by proposedShipmentID?, for not we are identifying shipment selections on xp
    // based on the shipfromaddressID
    const supplierID = this.getSupplierID(proposedShipment);
    const shipFromAddressID = this.getShipFromAddressID(proposedShipment);
    if (!this.order.xp) return null;
    const proposedShipmentSelection = this.order.xp.ProposedShipmentSelections
      .find(selection => selection.ShipFromAddressID === shipFromAddressID && selection.SupplierID === supplierID);
    return proposedShipmentSelection && proposedShipmentSelection.ProposedShipmentOptionID || null;
  }

  getSupplierID(proposedShipment: ProposedShipment): string {
    const firstLineItemID = proposedShipment.ProposedShipmentItems[0].LineItemID;
    const firstLineItem = this.lineItems.Items.find(lineItem => lineItem.ID === firstLineItemID);
    return firstLineItem.SupplierID;
  }

  getShipFromAddressID(proposedShipment: ProposedShipment): string {
    const firstLineItemID = proposedShipment.ProposedShipmentItems[0].LineItemID;
    const firstLineItem = this.lineItems.Items.find(lineItem => lineItem.ID === firstLineItemID);
    return firstLineItem.ShipFromAddressID;
  }

  selectRate(selection: ProposedShipmentSelection) {
    this.selectShipRate.emit(selection);
  }

  onContinueClicked() {
    this.continue.emit();
  }
}
