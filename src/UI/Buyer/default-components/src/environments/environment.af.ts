// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.

import { OrdercloudEnv } from 'marketplace';
import { Environment } from './env.interface';
import afTheme from '../styles/themes/anytime-fitness/theme-config';

export const environment: Environment = {
  hostedApp: true,
  appname: 'Anytime Fitness',
  clientID: 'F18AE28D-FFA4-4A5A-9C69-A1FBC71DCD3D',
  marketplaceID: 'SEB',
  baseUrl: 'http://marketplace-buyer-ui.azurewebsites.net/',
  middlewareUrl: 'https://marketplace-middleware.azurewebsites.net',
  translateBlobUrl: 'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
  ssoLink:
    'https://stage-authorize.anytimefitness.com/authorize?response_type=code&client_id=86d70db9-22e6-47ba-a1ab-bbe00c9b6451&redirect_uri=https://selfesteembrands-api-qa.azurewebsites.net/authorize',
  ordercloudEnv: OrdercloudEnv.Production,
  theme: afTheme,
};
/*

 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
