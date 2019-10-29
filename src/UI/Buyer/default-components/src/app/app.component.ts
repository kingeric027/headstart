import { Component, OnInit } from '@angular/core';
import { ShopperContextService } from 'marketplace';

@Component({
  selector: 'app-root',
  template: `
    <ocm-app-header class="fixed-top" [context]="context" *ngIf="showHeader"></ocm-app-header>
    <router-outlet></router-outlet>
    <ocm-app-footer [showFooter]="showHeader" *ngIf="showHeader"></ocm-app-footer>
    <ng-progress></ng-progress>
  `,
})
export class AppComponent implements OnInit {
  constructor(public context: ShopperContextService) {}

  showHeader = false;
  hiddenRoutes = ['/login', '/register', '/forgot-password', '/reset-password'];

  // TODO: this shouln't have hard coded routes. its gross.
  ngOnInit() {
    this.context.router.onUrlChange(url => {
      this.showHeader = !this.hiddenRoutes.some(el => url.indexOf(el) > -1);
    });
  }
}
