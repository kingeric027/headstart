import { Component } from '@angular/core';
import { ShopperContextService } from '../services/shopper-context/shopper-context.service';

@Component({
  template: `
    <ocm-change-password></ocm-change-password>
  `,
})
export class MeChangePasswordWrapperComponent {
  constructor(public context: ShopperContextService) {}
}
