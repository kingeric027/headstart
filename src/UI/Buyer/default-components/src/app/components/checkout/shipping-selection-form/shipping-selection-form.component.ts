import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ShipmentEstimate, ShipmentPreference } from 'marketplace';
import { FormGroup, FormControl } from '@angular/forms';

@Component({
  templateUrl: './shipping-selection-form.component.html',
  styleUrls: ['./shipping-selection-form.component.scss'],
})
export class OCMShippingSelectionForm implements OnInit {
  _shipmentEstimate: ShipmentEstimate;
  @Input() set shipmentEstimate(value: ShipmentEstimate) {
    this._shipmentEstimate = value;
    this.setSelectedRate(value.SelectedShipMethodID);
  }
  @Input() shipFromAddressID: string;
  @Input() supplierID: string;
  @Output() selectionChanged = new EventEmitter<ShipmentPreference>();

  form: FormGroup;

  constructor() {}

  ngOnInit(): void {
    this.form = new FormGroup({ rateID: new FormControl(null) });
    this.form = new FormGroup({ ShipmentMethodID: new FormControl(null) });
  }

  setSelectedRate(selectedShipmentMethodID: string): void {
    this.form.setValue({ ShipmentMethodID: selectedShipmentMethodID });
  }

  onFormChanges(): void {
    const selectedShipMethodID = this.form.value.ShipmentMethodID;
    this.selectionChanged.emit({
      ShipmentMethodID: selectedShipMethodID,
      ShipmentEstimateID: this._shipmentEstimate.ID,
    });
  }
}
