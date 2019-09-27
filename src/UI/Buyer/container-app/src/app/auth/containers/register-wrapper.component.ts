import { Component } from '@angular/core';
import { ShopperContextService } from './node_modules/src/app/shared/services/shopper-context/shopper-context.service';

@Component({
  template: '<ocm-register [context]="context"></ocm-register>',
})
export class RegisterWrapperComponent {
  constructor(public context: ShopperContextService) {}
}
