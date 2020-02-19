import { Component, OnInit } from '@angular/core';
import { ShopperContextService } from 'marketplace';

@Component({
  selector: 'app-root',
  template: `
    <ocm-app-header class="fixed-top" *ngIf="showHeader"></ocm-app-header>
    <router-outlet></router-outlet>
    <ocm-app-footer [showFooter]="showHeader" *ngIf="showHeader"></ocm-app-footer>
    <ng-progress></ng-progress>
  `,
})
export class AppComponent implements OnInit { 
  showHeader = false;
  // TODO: this shouln't have hard coded routes. its gross.
  hiddenRoutes = ['/login', '/register', '/forgot-password', '/reset-password'];
  
  constructor(public context: ShopperContextService) {}

  ngOnInit(): void {
    this.context.router.onUrlChange(url => {
      this.showHeader = !this.hiddenRoutes.some(el => url.includes(el));
    });
  }
}
