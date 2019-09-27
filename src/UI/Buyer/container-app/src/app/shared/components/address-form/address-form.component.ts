import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';

// 3rd party
import { BuyerAddress, Address } from '@ordercloud/angular-sdk';

import { AppGeographyService } from 'src/app/shared/services/geography/geography.service';
import { ValidateName, ValidateUSZip, ValidatePhone } from 'src/app/ocm-default-components/validators/validators';

@Component({
  selector: 'shared-address-form',
  templateUrl: './address-form.component.html',
  styleUrls: ['./address-form.component.scss'],
})
export class AddressFormComponent implements OnInit {
  private _existingAddress: BuyerAddress;
  @Input() btnText: string;
  @Output()
  formSubmitted = new EventEmitter<{ address: Address; formDirty: boolean }>();
  stateOptions: string[] = [];
  countryOptions: { label: string; abbreviation: string }[];
  addressForm: FormGroup;

  constructor(private geographyService: AppGeographyService) {
    this.countryOptions = this.geographyService.getCountries();
  }

  ngOnInit() {
    this.setForm();
  }

  @Input() set existingAddress(address: BuyerAddress) {
    this._existingAddress = address || {};
    this.setForm();
    this.addressForm.markAsPristine();
  }

  setForm() {
    this.addressForm = new FormGroup({
      FirstName: new FormControl(this._existingAddress.FirstName || '', [Validators.required, ValidateName]),
      LastName: new FormControl(this._existingAddress.LastName || '', [Validators.required, ValidateName]),
      Street1: new FormControl(this._existingAddress.Street1 || '', Validators.required),
      Street2: new FormControl(this._existingAddress.Street2 || ''),
      City: new FormControl(this._existingAddress.City || '', [Validators.required, ValidateName]),
      State: new FormControl(this._existingAddress.State || null, Validators.required),
      Zip: new FormControl(this._existingAddress.Zip || '', [Validators.required, ValidateUSZip]),
      Phone: new FormControl(this._existingAddress.Phone || '', ValidatePhone),
      Country: new FormControl(this._existingAddress.Country || 'US', Validators.required),
      ID: new FormControl(this._existingAddress.ID || ''),
    });
    this.onCountryChange();
  }

  onCountryChange(event?) {
    const country = this.addressForm.value.Country;
    this.stateOptions = this.geographyService.getStates(country).map((s) => s.abbreviation);
    this.addressForm.get('Zip').setValidators([Validators.required, ValidateUSZip]);
    if (event) {
      this.addressForm.patchValue({ State: null, Zip: '' });
    }
  }

  onSubmit() {
    if (this.addressForm.status === 'INVALID') {
      return;
    }
    this.formSubmitted.emit({
      address: this.addressForm.value,
      formDirty: this.addressForm.dirty,
    });
  }
}
