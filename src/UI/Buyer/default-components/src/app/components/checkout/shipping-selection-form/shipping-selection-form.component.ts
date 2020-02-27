import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ProposedShipment, ShipmentPreference } from 'marketplace';
import { FormGroup, FormControl } from '@angular/forms';

@Component({
  templateUrl: './shipping-selection-form.component.html',
  styleUrls: ['./shipping-selection-form.component.scss'],
})
export class OCMShippingSelectionForm implements OnInit {
  _proposedShipment: ProposedShipment;
  @Input() set proposedShipment(value: ProposedShipment) {
    this._proposedShipment = value;
    this.setSelectedRate(value.SelectedProposedShipmentOptionID);
  };
  @Input() shipFromAddressID: string;
  @Input() supplierID: string;
  @Output() selectionChanged = new EventEmitter<ShipmentPreference>();

  form: FormGroup;

  constructor() {}

  ngOnInit(): void {
    this.form = new FormGroup({ rateID: new FormControl(null) });
    this.form = new FormGroup({ proposedShipmentOptionID: new FormControl(null) });
  }

  setSelectedRate(selectedProposedShipmentOptionID: string): void {
    this.form.setValue({ proposedShipmentOptionID: selectedProposedShipmentOptionID });
  }

  onFormChanges(): void {
    const selectedProposedShipmentOptionID = this.form.value.proposedShipmentOptionID;
    this.selectionChanged.emit({
      ProposedShipmentOptionID: selectedProposedShipmentOptionID,
      ProposedShipmentID: this._proposedShipment.ID
    });
  }
}
