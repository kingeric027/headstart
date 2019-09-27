import { Component } from '@angular/core';
import { ShopperContextService } from './node_modules/src/app/shared/services/shopper-context/shopper-context.service';

@Component({
  template: '<ocm-forgot-password [context]="context"></ocm-forgot-password>',
})
export class ForgotPasswordWrapperComponent {
  constructor(public context: ShopperContextService) {}
}
