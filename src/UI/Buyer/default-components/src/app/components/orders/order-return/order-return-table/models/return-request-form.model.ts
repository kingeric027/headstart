import { FormArray, FormControl, FormBuilder } from '@angular/forms';
import { MarketplaceLineItem } from '@ordercloud/headstart-sdk';
import { LineItemGroupForm } from './line-item-group-form.model';

export class ReturnRequestForm {
  orderID = new FormControl();
  liGroups = new FormArray([]);

  constructor(private fb: FormBuilder, orderID: string, liGroups: MarketplaceLineItem[][], action: string) {
    if (orderID) {
      this.orderID.setValue(orderID);
    }
    liGroups.forEach(liGroup => this.liGroups.push(this.fb.group(new LineItemGroupForm(fb, liGroup, action))));
  }
}
