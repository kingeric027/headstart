import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ValidateEmail, ValidateName, ValidatePhone } from 'src/app/validators/validators';
import { QuoteOrderInfo } from 'marketplace-javascript-sdk';
import { CurrentUser } from 'marketplace';

@Component({
  templateUrl: './quote-request-form.component.html',
  styleUrls: ['./quote-request-form.component.scss'],
})
export class OCMQuoteRequestForm implements OnInit {
  quoteRequestForm: FormGroup;
  @Output() formSubmitted = new EventEmitter<{ user: QuoteOrderInfo }>();
  @Output() formDismissed = new EventEmitter();

  private currentUser: CurrentUser;
  constructor() {}

  ngOnInit(): void {
    this.setForms();
  }

  @Input() set CurrentUser(user: CurrentUser) {
    this.currentUser = user;
    this.setForms();
    this.quoteRequestForm.markAsPristine();
  }

  setForms(): void {
    this.quoteRequestForm = new FormGroup({
      FirstName: new FormControl(this.currentUser.FirstName || '', [Validators.required, ValidateName]),
      LastName: new FormControl(this.currentUser.LastName || '', [Validators.required, ValidateName]),
      // BuyerLocation: new FormControl(this.currentUser.BuyerLocation || '', [Validators.required, ValidateName]),
      Phone: new FormControl(this.currentUser.Phone || '', [Validators.required, ValidatePhone]),
      Email: new FormControl(this.currentUser.Email || '', [Validators.required, ValidateEmail]),
      Comments: new FormControl(''),
    });
  }

  onSubmit(): void {
    if (this.quoteRequestForm.status === 'INVALID') return;
    this.formSubmitted.emit({
      user: this.quoteRequestForm.value,
    });
  }

  dismissForm(): void {
    this.formDismissed.emit();
  }
}
