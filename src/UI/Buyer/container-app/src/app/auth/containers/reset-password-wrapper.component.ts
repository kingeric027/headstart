import { Component } from '@angular/core';
import { ShopperContextService } from './node_modules/src/app/shared/services/shopper-context/shopper-context.service';

@Component({
  templateUrl: '<ocm-reset-password [context]="context"></ocm-reset-password>',
})
export class ResetPasswordWrapperComponent {
  constructor(public context: ShopperContextService) {}
}
