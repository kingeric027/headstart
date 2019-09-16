import { Component, OnInit, Inject } from '@angular/core';
import { applicationConfiguration, AppConfig } from '@app-buyer/config/app.config';
import { Router } from '@angular/router';
import { ShopperContextService } from '@app-buyer/shared/services/shopper-context/shopper-context.service';

@Component({
  selector: 'app-header-wrapper',
  templateUrl: './header-wrapper.component.html',
  styleUrls: ['./header-wrapper.component.scss'],
})
export class HeaderWrapperComponent implements OnInit {
  constructor(@Inject(applicationConfiguration) public config: AppConfig, private router: Router, public context: ShopperContextService) {}

  ngOnInit() {}

  // TODO: this shouln't have hard coded routes. its gross.
  showHeader(): boolean {
    const hiddenRoutes = ['/login', '/register', '/forgot-password', '/reset-password'];
    return !hiddenRoutes.some((el) => this.router.url.indexOf(el) > -1);
  }
}
