import { Component } from '@angular/core';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';

@Component({
  template: `
    <ocm-payment-method-management [context]="context"></ocm-payment-method-management>
  `,
})
export class PaymentListWrapperComponent {
  constructor(public context: ShopperContextService) {}
}
