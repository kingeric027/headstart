import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';

// 3rd party
import { BuyerAddress, Address } from '@ordercloud/angular-sdk';

import { ValidateName, ValidateUSZip, ValidatePhone } from '../../validators/validators';
import { GeographyConfig } from '../../config/geography.class';

@Component({
  templateUrl: './address-form.component.html',
  styleUrls: ['./address-form.component.scss'],
})
export class OMCAddressForm implements OnInit {
  private ExistingAddress: BuyerAddress = {};
  @Input() btnText: string;
  @Output()
  formSubmitted = new EventEmitter<{ address: Address; formDirty: boolean }>();
  stateOptions: string[] = [];
  countryOptions: { label: string; abbreviation: string }[];
  addressForm: FormGroup;

  constructor() {
    this.countryOptions = GeographyConfig.getCountries();
  }

  ngOnInit() {
    this.setForm();
  }

  @Input() set existingAddress(address: BuyerAddress) {
    this.ExistingAddress = address || {};
    this.setForm();
    this.addressForm.markAsPristine();
  }

  setForm() {
    this.addressForm = new FormGroup({
      FirstName: new FormControl(this.ExistingAddress.FirstName || '', [Validators.required, ValidateName]),
      LastName: new FormControl(this.ExistingAddress.LastName || '', [Validators.required, ValidateName]),
      Street1: new FormControl(this.ExistingAddress.Street1 || '', Validators.required),
      Street2: new FormControl(this.ExistingAddress.Street2 || ''),
      City: new FormControl(this.ExistingAddress.City || '', [Validators.required, ValidateName]),
      State: new FormControl(this.ExistingAddress.State || null, Validators.required),
      Zip: new FormControl(this.ExistingAddress.Zip || '', [Validators.required, ValidateUSZip]),
      Phone: new FormControl(this.ExistingAddress.Phone || '', ValidatePhone),
      Country: new FormControl(this.ExistingAddress.Country || 'US', Validators.required),
      ID: new FormControl(this.ExistingAddress.ID || ''),
    });
    this.onCountryChange();
  }

  onCountryChange(event?) {
    const country = this.addressForm.value.Country;
    this.stateOptions = GeographyConfig.getStates(country).map(s => s.abbreviation);
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
