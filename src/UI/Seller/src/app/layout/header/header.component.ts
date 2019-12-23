import { Component, OnInit, Inject } from '@angular/core';
import { applicationConfiguration, AppConfig } from '@app-seller/config/app.config';
import {
  faBoxOpen,
  faSignOutAlt,
  faUser,
  faUsers,
  faMapMarkerAlt,
  faSitemap,
  faUserCircle,
} from '@fortawesome/free-solid-svg-icons';
import { OcTokenService } from '@ordercloud/angular-sdk';
import { Router } from '@angular/router';
import { AppStateService } from '@app-seller/shared';
import { getHeaderConfig, MPRoute } from './header.config';
import { AppAuthService } from '@app-seller/auth';

@Component({
  selector: 'layout-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
})
export class HeaderComponent implements OnInit {
  isCollapsed = true;
  faBoxOpen = faBoxOpen;
  faUser = faUser;
  faSignOutAlt = faSignOutAlt;
  faUsers = faUsers;
  faMapMarker = faMapMarkerAlt;
  faSitemap = faSitemap;
  faUserCircle = faUserCircle;
  activeTitle = '';
  headerConfig: MPRoute[];

  constructor(
    private ocTokenService: OcTokenService,
    private router: Router,
    private appStateService: AppStateService,
    private appAuthService: AppAuthService,
    @Inject(applicationConfiguration) protected appConfig: AppConfig
  ) {}

  ngOnInit() {
    this.headerConfig = getHeaderConfig(this.appAuthService.getUserRoles());
    this.subscribeToRouteEvents();
  }

  subscribeToRouteEvents() {
    this.router.events.subscribe(() => {
      const activeNavGroup = this.headerConfig.find((grouping) => {
        if (grouping.subRoutes) {
          return grouping.subRoutes.some((route) => this.router.url.includes(route.route));
        } else {
          return this.router.url.includes(grouping.route);
        }
      });
      this.activeTitle = activeNavGroup && activeNavGroup.title;
    });
  }

  logout() {
    this.ocTokenService.RemoveAccess();
    this.appStateService.isLoggedIn.next(false);
    this.router.navigate(['/login']);
  }
}

export interface Route {
  title: string;
  route: string;
}

export interface HeaderNav {
  title: string;
  routes: Route[];
}
