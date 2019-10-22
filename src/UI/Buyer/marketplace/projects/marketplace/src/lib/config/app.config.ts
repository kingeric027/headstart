import { InjectionToken } from '@angular/core';
import { AppConfig } from '../shopper-context';

const ocAppConfig: AppConfig = {
  appname: 'OrderCloud',
  anonymousShoppingEnabled: false,
  clientID: '97BBF2CC-59D1-449A-B67C-AE9262ADD284',
  baseUrl: 'http://localhost:33333',
  middlewareUrl: 'my-middleware-url.com/api',
  cmsUrl: 'https://s3.dualstack.us-east-1.amazonaws.com/staticcintas.eretailing.com/images/product',
  scope: ['MeAddressAdmin', 'MeAdmin', 'MeCreditCardAdmin', 'MeXpAdmin', 'Shopper', 'BuyerReader', 'PasswordReset'],
};

export const applicationConfiguration = new InjectionToken<AppConfig>('app.config', { providedIn: 'root', factory: () => ocAppConfig });
