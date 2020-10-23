// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.

import { OrdercloudEnv } from 'marketplace';
import { Environment } from './environment.interface';
import bcfTheme from '../styles/themes/basecamp/theme-config';

export const environment: Environment = {
  hostedApp: true,
  appname: 'Basecamp Fitness',
  clientID: '0045BB67-84CC-42BD-9FA7-009875628F7C',
  marketplaceID: 'SEB_TEST',
  baseUrl: 'https://basecampfitness-staging.sebvendorportal.com',
  middlewareUrl: 'https://marketplace-middleware-staging.azurewebsites.net',
  creditCardIframeUrl: 'https://fts-uat.cardconnect.com/itoke/ajax-tokenizer.html',
  translateBlobUrl: 'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
  sellerID: 'pPOiukEUHkSGrBmAIjdReQ',
  ssoLink: null,
  ordercloudEnv: OrdercloudEnv.Staging,
  theme: bcfTheme,
  instrumentationKey: '1feee80a-040f-4a3d-9aee-b618d6661756',
};
/*

 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
