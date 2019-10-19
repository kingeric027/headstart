import { Component, OnInit } from '@angular/core';
import { ShopperContextService } from 'marketplace';

@Component({
  selector: 'app-root',
  // template: `
  //           <ocm-app-header class="fixed-top" [showHeader]="showHeader"></ocm-app-header>
  //           <router-outlet></router-outlet>
  //           <ocm-app-footer [showFooter]="showHeader"></ocm-app-footer>
  //           <ng-progress></ng-progress><router-outlet></router-outlet>
  //           `,
  template: `
            <router-outlet></router-outlet>
            `,
})
export class AppComponent implements OnInit {
  constructor(private context: ShopperContextService) {}

  showHeader: boolean;
  hiddenRoutes = ['/login', '/register', '/forgot-password', '/reset-password'];

  // TODO: this shouln't have hard coded routes. its gross.
  ngOnInit() {
    this.context.router.onUrlChange(url => {
      this.showHeader = !this.hiddenRoutes.some((el) => url.indexOf(el) > -1);
    });
  }
}
