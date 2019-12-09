import { Component } from '@angular/core';
import { ShopperContextService } from '../services/shopper-context/shopper-context.service';

@Component({
  template: '<ocm-forgot-password></ocm-forgot-password>',
})
export class ForgotPasswordWrapperComponent {
  constructor(public context: ShopperContextService) {}
}
