import { Component } from '@angular/core';
import { ShopperContextService } from '../services/shopper-context/shopper-context.service';

@Component({
  selector: 'app-header-wrapper',
  template: `
    <ocm-app-header [context]="context"></ocm-app-header>
  `,
})
export class HeaderWrapperComponent {
  constructor(public context: ShopperContextService) {}
}
