import { InjectionToken } from '@angular/core';
import { environment } from '../../environments/environment';
import { AppConfig } from 'shopper-context-interface';

export const ocAppConfig: AppConfig = {
  appname: 'OrderCloud',
  clientID: environment.clientID,
  baseUrl: environment.baseUrl,
  anonymousShoppingEnabled: false,
  middlewareUrl: environment.middlewareUrl,
  cmsUrl: environment.cmsUrl,
  scope: ['MeAddressAdmin', 'MeAdmin', 'MeCreditCardAdmin', 'MeXpAdmin', 'Shopper', 'BuyerReader', 'PasswordReset'],
};

export const applicationConfiguration = new InjectionToken<AppConfig>('app.config', { providedIn: 'root', factory: () => ocAppConfig });
