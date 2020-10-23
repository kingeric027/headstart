// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.

import { OrdercloudEnv } from 'marketplace';
import { Environment } from './environment.interface';
import afTheme from '../styles/themes/anytime-fitness/theme-config';

export const environment: Environment = {
  hostedApp: true,
  appname: 'Anytime Fitness',
  clientID: 'F18AE28D-FFA4-4A5A-9C69-A1FBC71DCD3D',
  marketplaceID: 'SEB_TEST',
  baseUrl: 'https://anytimefitness-staging.sebvendorportal.com',
  middlewareUrl: 'https://marketplace-middleware-staging.azurewebsites.net',
  creditCardIframeUrl: 'https://fts-uat.cardconnect.com/itoke/ajax-tokenizer.html',
  translateBlobUrl: 'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
  sellerID: 'pPOiukEUHkSGrBmAIjdReQ',
  ssoLink:
    'https://stage-authorize.anytimefitness.com/authorize?response_type=code&client_id=7f9257b2-8a27-4527-9efa-225f4cb172da&redirect_uri=https://seb-four51-integration-stage.azurewebsites.net/api/anytime/authorize',
  ordercloudEnv: OrdercloudEnv.Staging,
  theme: afTheme,
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