import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { groupBy as _groupBy } from 'lodash';
import { ShopperContextService, LineItem, MarketplaceOrder, ProposedShipment, ProposedShipmentSelection } from 'marketplace';

@Component({
  templateUrl: './checkout-shipping.component.html',
  styleUrls: ['./checkout-shipping.component.scss']
})
export class OCMCheckoutShipping implements OnInit {
  @Output() continue = new EventEmitter();

  order: MarketplaceOrder;
  lineItems: LineItem[];
  proposedShipments: ProposedShipment[] = null;

  constructor(private context: ShopperContextService) {}

  async ngOnInit() {
    // placeholder to patch the shipping selections to an empty array
    this.order = await this.context.currentOrder.patch({ xp: { AvalaraTaxTransactionCode: '', ProposedShipmentSelections: [] }});
    this.lineItems = this.context.currentOrder.getLineItems().Items;
    this.proposedShipments = await this.context.currentOrder.getProposedShipments();
  }

  async onContinueClicked() {
    await this.context.currentOrder.calculateTax();
    this.continue.emit();
  }

  getLineItemsForProposedShipment(proposedShipment: ProposedShipment): LineItem[] {
    return proposedShipment.ProposedShipmentItems.map(proposedShipmentItem => {
      return this.lineItems.find(li => li.ID === proposedShipmentItem.LineItemID);
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
    const firstLineItem = this.lineItems.find(lineItem => lineItem.ID === firstLineItemID);
    return firstLineItem.SupplierID;
  }

  getShipFromAddressID(proposedShipment: ProposedShipment): string {
    const firstLineItemID = proposedShipment.ProposedShipmentItems[0].LineItemID;
    const firstLineItem = this.lineItems.find(lineItem => lineItem.ID === firstLineItemID);
    return firstLineItem.ShipFromAddressID;
  }

  async selectRate(selection: ProposedShipmentSelection) {
    this.order = await this.context.currentOrder.selectShippingRate(selection);
  }
}
