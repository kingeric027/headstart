import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ShipEstimate, ShipMethodSelection } from 'marketplace';
import { FormGroup, FormControl } from '@angular/forms';
import { faQuestionCircle } from '@fortawesome/free-solid-svg-icons';

@Component({
  templateUrl: './shipping-selection-form.component.html',
  styleUrls: ['./shipping-selection-form.component.scss'],
})
export class OCMShippingSelectionForm implements OnInit {
  faQuestionCircle = faQuestionCircle;
  _shipEstimate: ShipEstimate;
  @Input() set shipEstimate(value: ShipEstimate) {
    this._shipEstimate = value;
    console.log(value)
    this.setSelectedRate(value.SelectedShipMethodID);
  }
  @Input() shipFromAddressID: string;
  @Input() supplierID: string;
  @Output() selectionChanged = new EventEmitter<ShipMethodSelection>();

  form: FormGroup;

  constructor() { }

  ngOnInit(): void {
    this.form = new FormGroup({ rateID: new FormControl(null) });
    this.form = new FormGroup({ ShipMethodID: new FormControl(null) });
  }

  setSelectedRate(selectedShipMethodID: string): void {
    this.form.setValue({ ShipMethodID: selectedShipMethodID });
  }

  onFormChanges(): void {
    const selectedShipMethodID = this.form.value.ShipMethodID;
    this.selectionChanged.emit({
      ShipMethodID: selectedShipMethodID,
      ShipEstimateID: this._shipEstimate.ID,
    });
  }

  detectPlural(value: number): string {
    return value === 1 ? '' : 's';
  }
}
