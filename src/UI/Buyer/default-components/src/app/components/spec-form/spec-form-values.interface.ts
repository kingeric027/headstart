import { LineItemSpec, BuyerSpec, ListBuyerSpec } from '@ordercloud/angular-sdk';
import { FormGroup } from '@angular/forms';

export interface SpecFormEvent {
  type: string;
  valid: boolean;
  form: FormGroup;
  specs: ListBuyerSpec;
}
