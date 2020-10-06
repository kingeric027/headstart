import { Component, ViewEncapsulation } from '@angular/core';
import { ShopperContextService } from 'marketplace';
import { RouteConfig } from 'marketplace/projects/marketplace/src/lib/services/route/route-config';

@Component({
  templateUrl: './profile-nav.component.html',
  styleUrls: ['./profile-nav.component.scss'],
  encapsulation: ViewEncapsulation.ShadowDom,
})
export class OCMProfileNav {
  profileRoutes: RouteConfig[] = [];

  constructor(public context: ShopperContextService) {
    const isSSO = context.currentUser.isSSO() || !context.currentUser.hasRoles("PasswordReset");
    this.profileRoutes = context.router.getProfileRoutes();
    if (isSSO) this.profileRoutes = this.profileRoutes.filter(r => r.routerCall !== 'toChangePassword');
  }
}
