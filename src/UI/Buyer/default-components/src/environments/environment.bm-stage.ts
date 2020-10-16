// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.

import { OrdercloudEnv } from 'marketplace';
import { Environment } from './env.interface';
import bmTheme from '../styles/themes/bar-method/theme-config';

export const environment: Environment = {
  hostedApp: true,
  appname: 'The Bar Method',
  clientID: 'CF547B04-1826-427D-9940-824805F0ECA0',
  marketplaceID: 'SEB',
  baseUrl: 'https://sebrandsmarketplace-bar-stage.azurewebsites.net/',
  middlewareUrl: 'https://marketplace-middleware-staging.azurewebsites.net',
  creditCardIframeUrl: 'https://fts-uat.cardconnect.com/itoke/ajax-tokenizer.html',
  translateBlobUrl: 'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
  sellerID: 'pPOiukEUHkSGrBmAIjdReQ',
  ssoLink: null,
  ordercloudEnv: OrdercloudEnv.Staging,
  theme: bmTheme,
  instrumentationKey: '3a0b0eb7-9a02-4f97-b75d-c4811aec975e',
};
/*

 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
