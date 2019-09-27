import { Component } from '@angular/core';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';

@Component({
  template: '<ocm-login [context]="context"></ocm-login>',
})
export class LoginWrapperComponent {
  constructor(public context: ShopperContextService) {}
}
