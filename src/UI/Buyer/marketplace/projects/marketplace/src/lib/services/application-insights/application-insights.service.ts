import { Injectable } from '@angular/core';
import { AppConfig } from '../../shopper-context';
import { ApplicationInsights } from '@microsoft/applicationinsights-web';

@Injectable({
    providedIn: 'root',
  })
  export class ApplicationInsightsService {
    appInsights: ApplicationInsights = null;

    constructor(private appConfig: AppConfig) {
        if (this.appConfig.instrumentationKey) {
            this.appInsights = new ApplicationInsights({
                config: {
                  instrumentationKey: this.appConfig.instrumentationKey,
                  enableAutoRouteTracking: true,
                },
              });
              this.appInsights.loadAppInsights();
        }
    }

    public logException(exception: Error): void {
        if (this.appConfig.instrumentationKey) {
            this.appInsights.trackException({ exception });
        }
      }
  }