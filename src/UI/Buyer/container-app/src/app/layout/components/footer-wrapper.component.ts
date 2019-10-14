import { Component } from '@angular/core';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';

@Component({
  selector: 'app-footer-wrapper',
  template: `
    <ocm-app-footer [context]="context"></ocm-app-footer>
  `,
})
export class FooterWrapperComponent {
  constructor(public context: ShopperContextService) {}
}
