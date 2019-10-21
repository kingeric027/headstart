import { Component } from '@angular/core';
import { ShopperContextService } from '../services/shopper-context/shopper-context.service';

@Component({
  template: '<ocm-register [context]="context"></ocm-register>',
})
export class RegisterWrapperComponent {
  constructor(public context: ShopperContextService) {}
}
