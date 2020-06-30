import { Component, OnInit } from '@angular/core';
import { ShopperContextService } from 'marketplace';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
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
