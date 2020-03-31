import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ShopperContextService } from '../services/shopper-context/shopper-context.service';

@Component({
  template: `
    <ocm-user-management></ocm-user-management>
  `,
})
export class UserManagementWrapperComponent {
  constructor(public context: ShopperContextService, private activatedRoute: ActivatedRoute) {}
}
