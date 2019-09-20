import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { CreateCardDetails } from 'src/app/shared';
import { faPlus } from '@fortawesome/free-solid-svg-icons';
import { OCMComponent } from '../../shopper-context';

@Component({
  templateUrl: './credit-card-form.component.html',
  styleUrls: ['./credit-card-form.component.scss'],
})
export class OCMCreditCardForm extends OCMComponent implements OnInit {
  @Output() formSubmitted = new EventEmitter<CreateCardDetails>();
  cardForm: FormGroup;
  faPlus = faPlus;
  yearOptions: string[];
  monthOptions = ['01', '02', '03', '04', '05', '06', '07', '08', '09', '10', '11', '12'];

  ngOnInit() {
    const start = new Date().getFullYear();
    this.yearOptions = Array(20)
      .fill(0)
      .map((_x, i) => `${i + start}`);

    this.cardForm = new FormGroup({
      CardNumber: new FormControl('', Validators.required),
      CardholderName: new FormControl('', Validators.required),
      expMonth: new FormControl(this.monthOptions[0], Validators.required),
      expYear: new FormControl(this.yearOptions[0].slice(-2), Validators.required),
      CardCode: new FormControl('', Validators.required),
    });
  }

  onSubmit() {
    if (this.cardForm.status === 'INVALID') {
      return;
    }

    const date = `${this.cardForm.value.expMonth}${this.cardForm.value.expYear}`;
    const card = { ExpirationDate: date, ...this.cardForm.value };
    delete card.expMonth;
    delete card.expYear;
    this.formSubmitted.emit(card);
  }
}
