import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ProposedShipment, ProposedShipmentSelection } from 'marketplace';
import { FormGroup, FormControl } from '@angular/forms';

@Component({
  templateUrl: './shipping-selection-form.component.html',
  styleUrls: ['./shipping-selection-form.component.scss'],
})
export class OCMShippingSelectionForm implements OnInit {
  @Input() proposedShipment: ProposedShipment;
  @Input() shipFromAddressID: string;
  @Input() supplierID: string;
  @Input() set selectedProposedShipmentOptionID(value: string) {
    this.setSelectedRate(value);
  }
  @Output() selectionChanged = new EventEmitter<ProposedShipmentSelection>();

  form: FormGroup;

  constructor() {}

  ngOnInit() {
    this.form = new FormGroup({ rateID: new FormControl(null) });
    this.form = new FormGroup({ proposedShipmentOptionID: new FormControl(null) });
  }

  setSelectedRate(selectedProposedShipmentOptionID: string) {
    this.form.setValue({ proposedShipmentOptionID: selectedProposedShipmentOptionID });
  }

  onFormChanges() {
    const selectedProposedShipmentOptionID = this.form.value.proposedShipmentOptionID;
    this.selectionChanged.emit({
      SupplierID: this.supplierID,
      ShipFromAddressID: this.shipFromAddressID,
      ProposedShipmentOptionID: selectedProposedShipmentOptionID,
      Rate: this.proposedShipment.ProposedShipmentOptions
        .find(proposedShipment => proposedShipment.ID === selectedProposedShipmentOptionID).Cost
    });
  }
}
