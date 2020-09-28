import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ValidateEmail, ValidateName, ValidatePhone } from 'src/app/validators/validators';
import { QuoteOrderInfo, MarketplaceAddressBuyer } from '@ordercloud/headstart-sdk';
import { CurrentUser, ShopperContextService } from 'marketplace';

@Component({
  templateUrl: './quote-request-form.component.html',
  styleUrls: ['./quote-request-form.component.scss'],
})
export class OCMQuoteRequestForm implements OnInit {
  quoteRequestForm: FormGroup;
  requestOptions: {
    pageSize?: number;
  } = {
    pageSize: 100
  };
  myBuyerLocations: MarketplaceAddressBuyer[];
  @Output() formSubmitted = new EventEmitter<{ user: QuoteOrderInfo }>();
  @Output() formDismissed = new EventEmitter();

  private currentUser: CurrentUser;
  constructor(private context: ShopperContextService) {}

  async ngOnInit(): Promise<void> {
    await this.getMyBuyerLocations();
    this.setForms();
  }

  @Input() set CurrentUser(user: CurrentUser) {
    this.currentUser = user;
  }

  async getMyBuyerLocations(): Promise<void> {
    const addresses = await this.context.addresses.list(this.requestOptions);
    this.myBuyerLocations = addresses.Items;
  }

  setForms(): void {
    this.quoteRequestForm = new FormGroup({
      FirstName: new FormControl(this.currentUser?.FirstName || '', [Validators.required, ValidateName]),
      LastName: new FormControl(this.currentUser?.LastName || '', [Validators.required, ValidateName]),
      BuyerLocation: new FormControl(this.myBuyerLocations[0]?.AddressName || '', [Validators.required]),
      Phone: new FormControl(this.currentUser?.Phone || '', [Validators.required, ValidatePhone]),
      Email: new FormControl(this.currentUser?.Email || '', [Validators.required, ValidateEmail]),
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
