// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.

import { OrdercloudEnv } from 'marketplace';
import { Environment } from '../env.interface';
import bmTheme from '../../styles/themes/bar-method/theme-config';

export const environment: Environment = {
  hostedApp: true,
  appname: 'The Barre Method',
  clientID: '997F5753-4C11-4F5F-9B40-6AC843638BA2',
  marketplaceID: 'SEB',
  baseUrl: 'https://sebrandsmarketplace-bar-stage.azurewebsites.net/',
  middlewareUrl: 'https://marketplace-middleware-staging.azurewebsites.net',
  translateBlobUrl: 'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
  sellerID: 'rQYR6T6ZTEqVrgv8x_ei0g',
  ssoLink: null,
  ordercloudEnv: OrdercloudEnv.Production,
  theme: bmTheme,
};
/*

 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
