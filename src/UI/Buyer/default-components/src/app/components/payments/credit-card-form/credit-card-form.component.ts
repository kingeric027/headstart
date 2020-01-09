import { Component, OnInit, Output, EventEmitter, Input, ChangeDetectorRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
// import { AppFormErrorService } from '@app/shared/services/form-error/form-error.service';
import { faPlus } from '@fortawesome/free-solid-svg-icons';
import { faCcVisa, faCcMastercard, faCcDiscover, faCcPaypal } from '@fortawesome/free-brands-svg-icons';
import { faCheck, faTimes } from '@fortawesome/free-solid-svg-icons';
// import { AuthorizeNetService } from 'src/app/app.module';
import { AuthNetCreditCard } from 'marketplace';
import {
  RemoveSpacesFrom,
  IsValidCardType,
  IsValidPerLuhnAlgorithm,
  IsValidLength,
  GetCardType,
} from 'src/app/services/credit-card/credit-card.service';
import { ValidateCreditCard } from 'src/app/validators/validators';
import { CreditCardFormatPipe } from 'src/app/pipes/credit-card-format.pipe';

@Component({
  templateUrl: './credit-card-form.component.html',
  styleUrls: ['./credit-card-form.component.scss'],
})
export class OCMCreditCardForm implements OnInit {
  constructor(private formBuilder: FormBuilder, private creditCardFormatPipe: CreditCardFormatPipe) {}

  @Output() formSubmitted = new EventEmitter<AuthNetCreditCard>();
  @Output() formDismissed = new EventEmitter();
  @Input() card: AuthNetCreditCard;
  @Input() amount;
  @Input() submitText: string;
  cdr: ChangeDetectorRef;
  faCcVisa = faCcVisa;
  faCcMastercard = faCcMastercard;
  faCcDiscover = faCcDiscover;
  faCcPaypal = faCcPaypal;

  faCheck = faCheck;
  faTimes = faTimes;
  cardForm: FormGroup;
  faPlus = faPlus;
  yearOptions: string[];
  monthOptions: string[];
  cardNumberValid: boolean;
  cardType: string;
  notAcceptedCardType: boolean;

  ngOnInit() {
    this.setAvailableCardOptions();
    this.setCardForm();
  }

  handleCardNumberKeyup($event) {
    const cardNumber = RemoveSpacesFrom($event.target.value);
    this.setCardNumberVariables(cardNumber);
  }

  onSubmit() {
    // if (this.cardForm.status === 'INVALID') {
    //   this.formErrorService.displayFormErrors(this.cardForm);
    //   return;
    // }
    const date = `${this.cardForm.value.expMonth}${this.cardForm.value.expYear}`;

    let card = {
      ...this.cardForm.value,
      ExpirationDate: date,
      CardNumber: RemoveSpacesFrom(this.cardForm.value.CardNumber),
    };

    card = this.addTypeAndPartialNumber(card);
    delete card.expMonth;
    delete card.expYear;

    this.formSubmitted.emit(card);
  }

  handleYearChange($event) {
    const prevMonthValue = this.cardForm.value.expMonth;
    this.monthOptions = this.getArrayOfFutureMonthsForYear($event.target.value);

    // handles cases where the year is set to future years and then reset to the current year where the current month is not selectable
    // i.e. if it is Sept 2018, the user has March 2020 selected, the user resets to 2018, this resets the month to Sept.
    if (!this.monthOptions.includes(prevMonthValue)) {
      this.cardForm.value.expMonth = this.monthOptions[0];
    }
  }

  private getArrayOfFutureMonthsForYear(year: number): string[] {
    const monthArray: string[] = [];
    const slicedYear = Number(year.toString().slice(-2));
    if (
      slicedYear >
      Number(
        new Date()
          .getFullYear()
          .toString()
          .slice(-2)
      )
    ) {
      for (let i = 1; i <= 12; i++) {
        monthArray.push(`0${i}`.slice(-2));
      }
    } else {
      for (let i = new Date().getMonth() + 1; i <= 12; i++) {
        const a = `0${i}`;
        monthArray.push(a.slice(-2));
      }
    }
    return monthArray;
  }

  private setCardNumberVariables(cardNumber: string) {
    this.notAcceptedCardType = !IsValidCardType(cardNumber) && cardNumber.length > 5;
    this.cardNumberValid = IsValidPerLuhnAlgorithm(cardNumber) && IsValidLength(cardNumber);
    const cardType = GetCardType(cardNumber);
    this.cardType = GetCardType(cardNumber);
  }

  private addTypeAndPartialNumber(card) {
    return {
      ...card,
      CardType: GetCardType(card.CardNumber),
      PartialAccountNumber: card.CardNumber.substr(12),
    };
  }

  private setAvailableCardOptions() {
    const currentYear = new Date().getFullYear();
    this.yearOptions = Array(20)
      .fill(0)
      .map((x, i) => `${i + currentYear}`);
    const currentlySelectedYear = this.card ? parseInt(this.card.ExpirationDate.substring(2, 4), 10) : currentYear + 1; // sets the default year to one after the current year if there is none provided by the checkout state
    this.monthOptions = this.getArrayOfFutureMonthsForYear(currentlySelectedYear);
  }

  private setCardForm() {
    if (this.card && this.card.CardNumber) {
      const expMonth = this.card.ExpirationDate.substring(0, 2);
      const expYear = this.card.ExpirationDate.substring(2, 4);
      this.cardForm = this.formBuilder.group({
        CardNumber: [
          this.creditCardFormatPipe.transform(this.card.CardNumber),
          [Validators.required, ValidateCreditCard],
        ],
        CardholderName: [this.card.CardholderName, Validators.required],
        expMonth: [expMonth, Validators.required],
        expYear: [expYear, Validators.required],
        SecurityCode: [this.card.SecurityCode, Validators.required],
      });
      this.setCardNumberVariables(this.card.CardNumber);
    } else {
      this.cardForm = this.formBuilder.group({
        CardNumber: ['', [Validators.required, ValidateCreditCard]],
        CardholderName: ['', Validators.required],
        expMonth: [this.monthOptions[0], Validators.required],
        expYear: [this.yearOptions[1].slice(-2), Validators.required],
        SecurityCode: ['', Validators.required],
      });
    }
  }

  dismissForm() {
    this.formDismissed.emit();
  }
}
