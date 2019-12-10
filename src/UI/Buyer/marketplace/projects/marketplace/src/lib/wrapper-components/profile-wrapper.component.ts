import { Component } from '@angular/core';
import { ShopperContextService } from '../services/shopper-context/shopper-context.service';

@Component({
  template: `
    <ocm-profile></ocm-profile>
  `,
})
export class ProfileWrapperComponent {
  constructor(public context: ShopperContextService) {}
}
