import { Component } from '@angular/core';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';

@Component({
  template: '<ocm-checkout [context]="context"></ocm-checkout>',
})
export class CheckoutWrapperComponent {
  constructor(public context: ShopperContextService) {}
}
