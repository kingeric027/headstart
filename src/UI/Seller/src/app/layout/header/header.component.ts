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
import { Router, NavigationEnd } from '@angular/router';
import { AppStateService } from '@app-seller/shared';
import { getHeaderConfig, MPRoute } from './header.config';
import { AppAuthService } from '@app-seller/auth';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';

@Component({
  selector: 'layout-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
})
export class HeaderComponent implements OnInit {
  user = {};
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
    private currentUserService: CurrentUserService,
    @Inject(applicationConfiguration) protected appConfig: AppConfig
  ) {}

  ngOnInit() {
    this.headerConfig = getHeaderConfig(this.appAuthService.getUserRoles());
    this.getCurrentUser();
    this.subscribeToRouteEvents();
    this.urlChange(this.router.url);
  }

  async getCurrentUser() {
    this.user = await this.currentUserService.getUser();
  }

  subscribeToRouteEvents() {
    this.router.events.subscribe(ev => {
      if (ev instanceof NavigationEnd) {
        this.urlChange(ev.url);
      }
    });
  }

  urlChange = (url: string) => {
    const activeNavGroup = this.headerConfig.find(grouping => {
      return url.includes(grouping.route);
    });
    this.activeTitle = activeNavGroup && activeNavGroup.title;
  };

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
