import { Component } from '@angular/core';
import { ShopperContextService } from '../services/shopper-context/shopper-context.service';

@Component({
  template: '<ocm-forgot-password [context]="context"></ocm-forgot-password>',
})
export class ForgotPasswordWrapperComponent {
  constructor(public context: ShopperContextService) {}
}
