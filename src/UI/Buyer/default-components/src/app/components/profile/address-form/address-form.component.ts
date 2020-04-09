import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';

// 3rd party
import { BuyerAddress, Address, ListBuyerAddress } from '@ordercloud/angular-sdk';
import { ValidateName, ValidateUSZip, ValidatePhone } from '../../../validators/validators';
import { GeographyConfig } from '../../../config/geography.class';

@Component({
  templateUrl: './address-form.component.html',
  styleUrls: ['./address-form.component.scss'],
})
export class OCMAddressForm implements OnInit {
  @Input() btnText: string;
  @Input() suggestedAddresses: BuyerAddress[];
  @Input() showOptionToSave = false;
  @Output() formDismissed = new EventEmitter();
  @Output()
  formSubmitted = new EventEmitter<{ address: Address; formDirty: boolean; shouldSaveAddress: boolean }>();
  stateOptions: string[] = [];
  countryOptions: { label: string; abbreviation: string }[];
  addressForm: FormGroup;
  shouldSaveAddressForm: FormGroup;
  private ExistingAddress: BuyerAddress = {};
  selectedAddress: BuyerAddress;

  constructor() {
    this.countryOptions = GeographyConfig.getCountries();
  }

  ngOnInit(): void {
    this.setForms();
  }

  @Input() set existingAddress(address: BuyerAddress) {
    this.ExistingAddress = address || {};
    this.setForms();
    this.addressForm.markAsPristine();
  }

  setForms(): void {
    this.addressForm = new FormGroup({
      FirstName: new FormControl(this.ExistingAddress.FirstName || '', [Validators.required, ValidateName]),
      LastName: new FormControl(this.ExistingAddress.LastName || '', [Validators.required, ValidateName]),
      Street1: new FormControl(this.ExistingAddress.Street1 || '', Validators.required),
      Street2: new FormControl(this.ExistingAddress.Street2 || ''),
      City: new FormControl(this.ExistingAddress.City || ''),
      State: new FormControl(this.ExistingAddress.State || null, Validators.required),
      Zip: new FormControl(this.ExistingAddress.Zip || '', [Validators.required, ValidateUSZip]),
      Phone: new FormControl(this.ExistingAddress.Phone || '', ValidatePhone),
      Country: new FormControl(this.ExistingAddress.Country || 'US', Validators.required),
      ID: new FormControl(this.ExistingAddress.ID || ''),
    });
    this.shouldSaveAddressForm = new FormGroup({
      shouldSaveAddress: new FormControl(false),
    });
    this.onCountryChange();
  }

  onCountryChange(event?): void {
    const country = this.addressForm.value.Country;
    this.stateOptions = GeographyConfig.getStates(country).map(s => s.abbreviation);
    this.addressForm.get('Zip').setValidators([Validators.required, ValidateUSZip]);
    if (event) {
      this.addressForm.patchValue({ State: null, Zip: '' });
    }
  }

  useSuggestedAddress(event) {
    this.selectedAddress = event.detail;
  }

  onSubmit(): void {
    if (this.addressForm.status === 'INVALID') return;
    this.formSubmitted.emit({
      address: this.selectedAddress ? this.selectedAddress : this.addressForm.value,
      formDirty: this.addressForm.dirty,
      shouldSaveAddress: this.shouldSaveAddressForm.controls.shouldSaveAddress.value,
    });
  }

  dismissForm(): void {
    this.formDismissed.emit();
  }
}
