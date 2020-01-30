import { Component, OnInit, Output, EventEmitter, Input, ChangeDetectorRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { CreditCardToken } from 'marketplace';
import { CreditCardFormatPipe } from 'src/app/pipes/credit-card-format.pipe';

@Component({
  templateUrl: './credit-card-form.component.html',
  styleUrls: ['./credit-card-form.component.scss'],
})
export class OCMCreditCardForm implements OnInit {
  constructor(private formBuilder: FormBuilder, private creditCardFormatPipe: CreditCardFormatPipe) {}

  @Output() formSubmitted = new EventEmitter<CreditCardToken>();
  @Output() formDismissed = new EventEmitter();
  @Input() card: CreditCardToken;
  @Input() submitText: string;
  
  cardForm: FormGroup;
  monthOptions = ['01', '02', '03', '04', '05', '06', '07', '08', '09', '10', '11', '12'];
  yearOptions = this.getYearOptions();

  ngOnInit() {
    this.setCardForm(this.card);
  }

  onSubmit() {
    this.formSubmitted.emit({
      AccountNumber: this.cardForm.value.CardNumber,
      CardholderName: this.cardForm.value.CardholderName,
      ExpirationDate: `${this.cardForm.value.expMonth}${this.cardForm.value.expYear}`,
    });
  }

  private setCardForm(card: CreditCardToken) {
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
    this.cardForm = this.formBuilder.group({
      CardNumber: [cardNumber, Validators.required],
      CardholderName: [cardholderName, Validators.required],
      expMonth: [expMonth, Validators.required],
      expYear: [expYear, Validators.required],
      SecurityCode: [''],
    });
  }

  private getYearOptions(): string[] {
    const currentYear = new Date().getFullYear();
    return Array(20).fill(0).map((x, i) => `${i + currentYear}`);
  }

  dismissForm() {
    this.formDismissed.emit();
  }
}
