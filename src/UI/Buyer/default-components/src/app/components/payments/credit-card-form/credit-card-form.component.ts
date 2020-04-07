import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { CreditCardFormatPipe } from 'src/app/pipes/credit-card-format.pipe';
import { ValidateCreditCard, ValidateUSZip } from 'src/app/validators/validators';
import { removeSpacesFrom } from 'src/app/services/card-validation.helper';
import { CreditCardToken } from 'marketplace-javascript-sdk';
import { GeographyConfig } from 'src/app/config/geography.class';

export interface CreditCardFormOutput {
  card: CreditCardToken;
  cvv: string;
}

@Component({
  templateUrl: './credit-card-form.component.html',
  styleUrls: ['./credit-card-form.component.scss'],
})
export class OCMCreditCardForm implements OnInit {
  @Output() formSubmitted = new EventEmitter<CreditCardFormOutput>();
  @Output() formDismissed = new EventEmitter();
  @Input() card: CreditCardToken;
  @Input() submitText: string;
  @Input() set showCVV(value) {
    if (value && !this._showCVV) {
      this.buildCVVForm();
    }
    if (!value && this._showCVV) {
      this.removeCVVForm();
    }
    this._showCVV = value;
  }
  @Input() set showCardDetails(value) {
    if (value && !this._showCardDetails) {
      this.buildCardDetailsForm(this.card);
    }
    if (!value && this._showCardDetails) {
      this.removeCardDetailsForm();
    }
    this._showCardDetails = value;
  }

  _showCVV = false;
  _showCardDetails = true;
  cardForm = new FormGroup({});
  monthOptions = ['01', '02', '03', '04', '05', '06', '07', '08', '09', '10', '11', '12'];
  yearOptions = this.buildYearOptions();
  stateOptions: string[] = [];
  countryOptions: { label: string; abbreviation: string }[];
  private readonly defaultCountry = 'US';

  constructor(private creditCardFormatPipe: CreditCardFormatPipe) {
    this.countryOptions = GeographyConfig.getCountries();
    this.stateOptions = this.getStateOptions(this.defaultCountry);
  }

  ngOnInit(): void {
    this.buildCardDetailsForm(this.card);
  }

  onSubmit(): void {
    this.formSubmitted.emit({
      card: {
        AccountNumber: removeSpacesFrom(this.cardForm.value.number || ''),
        CardholderName: this.cardForm.value.name,
        ExpirationDate: `${this.cardForm.value.month}${this.cardForm.value.year}`,
        CCBillingAddress: {
          Street1: this.cardForm.value.street,
          City: this.cardForm.value.city,
          State: this.cardForm.value.state,
          Zip: this.cardForm.value.zip,
          Country: this.cardForm.value.country,
        },
      },
      cvv: this.cardForm.value.cvv,
    });
  }

  dismissForm(): void {
    this.formDismissed.emit();
  }

  onCountryChange(event?): void {
    this.stateOptions = this.getStateOptions(this.cardForm.value.country);
    this.cardForm.get('zip').setValidators([Validators.required, ValidateUSZip]);
    if (event) {
      this.cardForm.patchValue({ State: null, Zip: '' });
    }
  }

  private buildCVVForm(): void {
    this.cardForm.addControl('cvv', new FormControl('', Validators.required));
  }

  private removeCVVForm(): void {
    this.cardForm.removeControl('cvv');
  }

  private buildCardDetailsForm(card: CreditCardToken): void {
    const form = {
      name: card?.CardholderName || '',
      number: card?.AccountNumber ?  this.creditCardFormatPipe.transform(card.AccountNumber) : '',
      month: card?.ExpirationDate?.substring(0, 2) || this.monthOptions[0],
      year: card?.ExpirationDate?.substring(2, 4) || this.yearOptions[1].slice(-2),
      street: card?.CCBillingAddress?.Street1 || '',
      city: card?.CCBillingAddress?.City || '',
      state: card?.CCBillingAddress?.State || null,
      zip: card?.CCBillingAddress?.Zip || '',
      country: card?.CCBillingAddress?.Country || this.defaultCountry
    }

    this.cardForm.addControl('number', new FormControl(form.number, [Validators.required, ValidateCreditCard]));
    this.cardForm.addControl('name', new FormControl(name, Validators.required));
    this.cardForm.addControl('month', new FormControl(form.month, Validators.required));
    this.cardForm.addControl('year', new FormControl(form.year, Validators.required));
    this.cardForm.addControl('street', new FormControl(form.street, Validators.required));
    this.cardForm.addControl('city', new FormControl(form.city, Validators.required));
    this.cardForm.addControl('state', new FormControl(form.state, Validators.required));
    this.cardForm.addControl('zip', new FormControl(form.zip, [Validators.required, ValidateUSZip]));
    this.cardForm.addControl('country', new FormControl(form.country, Validators.required));
  }

  private removeCardDetailsForm(): void {
    const nonCVVCtrls = [
      'name',
      'number',
      'month',
      'year',
      'street',
      'city',
      'state',
      'zip',
      'country',
    ];
    for (const ctrl of nonCVVCtrls) {
      this.cardForm.removeControl(ctrl);
    }
  }

  private getStateOptions(country: string): string[] {
    return GeographyConfig.getStates(country).map(s => s.abbreviation);
  }

  private buildYearOptions(): string[] {
    const currentYear = new Date().getFullYear();
    return Array(20)
      .fill(0)
      .map((x, i) => `${i + currentYear}`);
  }
}
