// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.

import { OrdercloudEnv } from 'marketplace';
import { Environment } from './env.interface';
import afTheme from '../styles/themes/anytime-fitness/theme-config';

export const environment: Environment = {
  hostedApp: false,
  appname: 'Anytime Fitness',
  clientID: 'E3A7326B-CACF-4F33-B894-0BCB40A19442',
  marketplaceID: 'SEB',
  baseUrl: 'https://localhost:4200',
  middlewareUrl: 'http://localhost:52059',
  translateBlobUrl: 'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
  ssoLink:
    'https://stage-authorize.anytimefitness.com/authorize?response_type=code&client_id=86d70db9-22e6-47ba-a1ab-bbe00c9b6451&redirect_uri=https://selfesteembrands-api-qa.azurewebsites.net/authorize',
  ordercloudEnv: OrdercloudEnv.Staging,
  sellerID: 'rQYR6T6ZTEqVrgv8x_ei0g',
  theme: afTheme,
};
//clientID: '0A53B491-1A25-4FCB-B3F1-E733A7490835',
/*

 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
