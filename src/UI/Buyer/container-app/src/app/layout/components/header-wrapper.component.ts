import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';

@Component({
  selector: 'app-header-wrapper',
  template: `
    <ocm-app-header [context]="context" [showHeader]="showHeader()"></ocm-app-header>
  `,
})
export class HeaderWrapperComponent implements OnInit {
  constructor(private router: Router, public context: ShopperContextService) {}

  ngOnInit() {}

  // TODO: this shouln't have hard coded routes. its gross.
  showHeader(): boolean {
    const hiddenRoutes = ['/login', '/register', '/forgot-password', '/reset-password'];
    return !hiddenRoutes.some((el) => this.router.url.indexOf(el) > -1);
  }
}
