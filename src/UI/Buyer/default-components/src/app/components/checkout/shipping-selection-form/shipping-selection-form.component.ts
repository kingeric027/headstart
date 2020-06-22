import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ShopperContextService } from 'marketplace';
import { ShipEstimate, ShipMethodSelection } from 'ordercloud-javascript-sdk';
import { FormGroup, FormControl } from '@angular/forms';
import { faQuestionCircle } from '@fortawesome/free-solid-svg-icons';

@Component({
  templateUrl: './shipping-selection-form.component.html',
  styleUrls: ['./shipping-selection-form.component.scss'],
})
export class OCMShippingSelectionForm implements OnInit {
  faQuestionCircle = faQuestionCircle;
  _shipEstimate: ShipEstimate;
  _orderCurrency: string;
  @Input() set shipEstimate(value: ShipEstimate) {
    this._shipEstimate = value;
    this.setSelectedRate(value.SelectedShipMethodID);
  }
  @Input() shipFromAddressID: string;
  @Input() supplierID: string;
  @Output() selectionChanged = new EventEmitter<ShipMethodSelection>();

  form: FormGroup;

  constructor(private context: ShopperContextService) {}

  ngOnInit(): void {
    this.form = new FormGroup({ rateID: new FormControl(null) });
    this.form = new FormGroup({ ShipMethodID: new FormControl(null) });
    this._orderCurrency = this.context.currentUser.get().Currency;
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
