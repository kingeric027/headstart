import { Component } from '@angular/core';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';

@Component({
  template: `
    <ocm-profile [context]="context"></ocm-profile>
  `,
})
export class ProfileWrapperComponent {
  constructor(public context: ShopperContextService) {}
}
