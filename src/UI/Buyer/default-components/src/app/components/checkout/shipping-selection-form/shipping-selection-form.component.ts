import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ShippingOptions, ShippingSelection } from 'marketplace';
import { FormGroup, FormControl } from '@angular/forms';

@Component({
  templateUrl: './shipping-selection-form.component.html',
  styleUrls: ['./shipping-selection-form.component.scss']
})
export class OCMShippingSelectionForm implements OnInit {
  @Input() options: ShippingOptions;
  @Input() set existingSelection(value: ShippingSelection) {
    this.setSelectedRate(value);
  }
  @Output() selectionChanged = new EventEmitter<ShippingSelection>();

  form: FormGroup;

  constructor() { }

  ngOnInit() {
    this.form = new FormGroup({  rateID: new FormControl(null) });
  }

  setSelectedRate(selection: ShippingSelection) {
    this.form.setValue({ rateID: selection.ShippingRateID });
  }

  onFormChanges() {
    this.selectionChanged.emit({
      ShipFromAddressID: this.options.ShipFromAddressID,
      SupplierID: this.options.SupplierID, 
      ShippingRateID: this.form.value.rateID
    });
  }
}
