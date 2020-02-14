import { Component, OnInit, Output, EventEmitter, Input, ChangeDetectorRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { CreditCardToken } from 'marketplace';
import { CreditCardFormatPipe } from 'src/app/pipes/credit-card-format.pipe';
import { ValidateCreditCard } from 'src/app/validators/validators';
import { removeSpacesFrom } from 'src/app/services/card-validation.helper';

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
      this.buildCardDetailsForm();
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
  yearOptions = this.getYearOptions();

  constructor(private creditCardFormatPipe: CreditCardFormatPipe) {}

  ngOnInit(): void {
    this.buildCardDetailsForm();
    this.setCardDetailsForm(this.card);
  }

  onSubmit(): void {
    this.formSubmitted.emit({
      card: {
        AccountNumber: removeSpacesFrom(this.cardForm.value.cardNumber || ''),
        CardholderName: this.cardForm.value.cardholderName,
        ExpirationDate: `${this.cardForm.value.expMonth}${this.cardForm.value.expYear}`,
      },
      cvv: this.cardForm.value.cvv,
    });
  }

  buildCVVForm(): void {
    this.cardForm.addControl('cvv', new FormControl('', Validators.required));
  }

  removeCVVForm(): void {
    this.cardForm.removeControl('cvv');
  }

  buildCardDetailsForm(): void {
    this.cardForm.addControl('cardNumber', new FormControl('', [Validators.required, ValidateCreditCard]));
    this.cardForm.addControl('cardholderName', new FormControl('', Validators.required));
    this.cardForm.addControl('expMonth', new FormControl('', Validators.required));
    this.cardForm.addControl('expYear', new FormControl('', Validators.required));
  }

  removeCardDetailsForm(): void {
    this.cardForm.removeControl('cardNumber');
    this.cardForm.removeControl('cardholderName');
    this.cardForm.removeControl('expMonth');
    this.cardForm.removeControl('expYear');
  }

  dismissForm(): void {
    this.formDismissed.emit();
  }

  private getYearOptions(): string[] {
    const currentYear = new Date().getFullYear();
    return Array(20)
      .fill(0)
      .map((x, i) => `${i + currentYear}`);
  }

  private setCardDetailsForm(card: CreditCardToken): void {
    let expMonth, expYear, cardNumber, cardholderName;
    if (card && card.AccountNumber) {
      expMonth = card.ExpirationDate.substring(0, 2);
      expYear = card.ExpirationDate.substring(2, 4);
      cardNumber = this.creditCardFormatPipe.transform(card.AccountNumber);
      cardholderName = card.CardholderName;
    } else {
      expMonth = this.monthOptions[0];
      expYear = this.yearOptions[1].slice(-2);
      cardNumber = '';
      cardholderName = '';
    }
    this.cardForm.setValue({ expMonth, expYear, cardNumber, cardholderName });
  }
}
