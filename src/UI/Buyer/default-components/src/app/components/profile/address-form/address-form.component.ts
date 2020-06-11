import { Component, OnInit, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';

// 3rd party
import { BuyerAddress, Address } from '@ordercloud/angular-sdk';
import { ValidateName, ValidateUSZip, ValidatePhone } from '../../../validators/validators';
import { GeographyConfig } from '../../../config/geography.class';

@Component({
  templateUrl: './address-form.component.html',
  styleUrls: ['./address-form.component.scss'],
})
export class OCMAddressForm implements OnInit, OnChanges {
  @Input() btnText: string;
  @Input() suggestedAddresses: BuyerAddress[];
  @Input() showOptionToSave = false;
  @Input() homeCountry: string;
  @Output() formDismissed = new EventEmitter();
  @Output()
  formSubmitted = new EventEmitter<{ address: Address; shouldSaveAddress: boolean }>();
  stateOptions: string[] = [];
  countryOptions: { label: string; abbreviation: string }[];
  addressForm: FormGroup;
  shouldSaveAddressForm: FormGroup;
  selectedAddress: BuyerAddress;
  private ExistingAddress: BuyerAddress = {};

  constructor() {
    this.countryOptions = GeographyConfig.getCountries();
  }

  ngOnInit(): void {
    this.setForms();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.homeCountry) {
      this.addressForm.controls.Country.setValue(this.homeCountry);
    }
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
      Country: new FormControl(this.homeCountry || '', Validators.required),
      ID: new FormControl(this.ExistingAddress.ID || ''),
    });
    this.shouldSaveAddressForm = new FormGroup({
      shouldSaveAddress: new FormControl(false),
    });
    this.onCountryChange();
  }

  onCountryChange(event?: any): void {
    const country = this.addressForm.value.Country;
    this.stateOptions = GeographyConfig.getStates(country).map(s => s.abbreviation);
    this.addressForm.get('Zip').setValidators([Validators.required, ValidateUSZip]);
    if (event) {
      this.addressForm.patchValue({ State: null, Zip: '' });
    }
  }

  getCountryName(countryCode: string): string {
    const country = this.countryOptions.find(country => country.abbreviation === countryCode);
    return country ? country.label : '';
  }

  useSuggestedAddress(address: BuyerAddress): void {
    this.selectedAddress = address;
  }

  onSubmit(): void {
    if (this.addressForm.status === 'INVALID') return;
    this.formSubmitted.emit({
      address: this.selectedAddress ? this.selectedAddress : this.addressForm.value,
      shouldSaveAddress: this.shouldSaveAddressForm.controls.shouldSaveAddress.value,
    });
  }

  dismissForm(): void {
    this.formDismissed.emit();
  }
}
