import { InjectionToken } from '@angular/core';
import { AppConfig } from '../shopper-context';

const ocAppConfig: AppConfig = {
  appname: 'OrderCloud',
  anonymousShoppingEnabled: false,
  clientID: '78F16865-A4C3-4D28-832D-A0371A93F1EA',
  baseUrl: 'http://localhost:33333',
  middlewareUrl: 'my-middleware-url.com/api',
  cmsUrl: 'https://s3.dualstack.us-east-1.amazonaws.com/staticcintas.eretailing.com/images/product',
  scope: ['MeAddressAdmin', 'MeAdmin', 'MeCreditCardAdmin', 'MeXpAdmin', 'Shopper', 'BuyerReader', 'PasswordReset'],
};

export const applicationConfiguration = new InjectionToken<AppConfig>('app.config', { providedIn: 'root', factory: () => ocAppConfig });
