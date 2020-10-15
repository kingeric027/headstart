import { Injectable } from '@angular/core';
import { AppConfig } from '../../shopper-context';
import { ApplicationInsights } from '@microsoft/applicationinsights-web';
import { ActivatedRouteSnapshot, ResolveEnd, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';

@Injectable({
    providedIn: 'root',
  })
  export class ApplicationInsightsService {
    appInsights: ApplicationInsights = null;
    routerSubscription: Subscription;

    constructor(
      private appConfig: AppConfig,
      private router: Router
    ) {
        if (this.appConfig.instrumentationKey) {
          this.appInsights = new ApplicationInsights({
            config: {
              instrumentationKey: this.appConfig.instrumentationKey,
              enableAutoRouteTracking: true,
            },
          });
          this.appInsights.loadAppInsights();
          this.createRouterSubscription();
        }
    }

    public setUserID(userID: string): void {
      if (this.appInsights) {
        this.appInsights.setAuthenticatedUserContext(userID);
      }
    }
  
    public clearUser(): void {
      if (this.appInsights) {
        this.appInsights.clearAuthenticatedUserContext();
      }
    }

    private createRouterSubscription(): void
    {
      this.router.events.pipe(filter(event => event instanceof ResolveEnd)).subscribe((event: ResolveEnd) => {
        const activatedComponent = this.getActivatedComponent(event.state.root)
        if (activatedComponent) {
          this.logPageView(
            `${activatedComponent.name} ${this.getRouteTemplate(
              event.state.root
            )}`,
            event.urlAfterRedirects
          );
        } else {
          this.logPageView(null, event.urlAfterRedirects);
        }
      });
    }

    private logPageView(name?: string, uri?: string): void {
      this.appInsights.trackPageView({ name, uri });
    }

    private getActivatedComponent(snapshot: ActivatedRouteSnapshot): any {
      if (snapshot.firstChild) {
        return this.getActivatedComponent(snapshot.firstChild);
      }
      return snapshot.component;
    }

    private getRouteTemplate(snapshot: ActivatedRouteSnapshot): string {
      let path = '';
      if (snapshot.routeConfig) {
        path += snapshot.routeConfig.path;
      }
      if (snapshot.firstChild) {
        return path + this.getRouteTemplate(snapshot.firstChild);
      }
      return path;
    }
  

  }