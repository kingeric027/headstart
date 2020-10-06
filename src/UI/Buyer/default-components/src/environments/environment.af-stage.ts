// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.

import { OrdercloudEnv } from 'marketplace';
import { Environment } from './env.interface';
import afTheme from '../styles/themes/anytime-fitness/theme-config';

export const environment: Environment = {
  hostedApp: true,
  appname: 'Anytime Fitness',
  clientID: 'ACE199F7-CC6D-403A-B833-9B0C8D84E7C8',
  marketplaceID: 'SEB',
  baseUrl: 'https://sebrandsmarketplace-anytime-stage.azurewebsites.net/',
  middlewareUrl: 'https://marketplace-middleware-staging.azurewebsites.net',
  translateBlobUrl: 'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
  sellerID: 'rQYR6T6ZTEqVrgv8x_ei0g',
  ssoLink:
    'https://stage-authorize.anytimefitness.com/authorize?response_type=code&client_id=7f9257b2-8a27-4527-9efa-225f4cb172da&redirect_uri=https://seb-four51-integration-stage.azurewebsites.net/api/anytime/authorize',
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
