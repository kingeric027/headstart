import { Component } from '@angular/core';
import { ShopperContextService } from '../services/shopper-context/shopper-context.service';

@Component({
  template: '<ocm-reset-password [context]="context"></ocm-reset-password>',
})
export class ResetPasswordWrapperComponent {
  constructor(public context: ShopperContextService) {}
}
