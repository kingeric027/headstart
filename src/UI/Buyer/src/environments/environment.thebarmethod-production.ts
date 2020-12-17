// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.

import { Environment } from './environment.interface'
import bmTheme from '../styles/themes/bar-method/theme-config'
import { OrdercloudEnv } from 'src/app/shopper-context'

export const environment: Environment = {
  hostedApp: true,
  appname: 'The Bar Method',
  clientID: 'CF547B04-1826-427D-9940-824805F0ECA0',
  marketplaceID: 'SEB',
  baseUrl: 'https://thebarmethod.sebvendorportal.com',
  middlewareUrl: 'https://middleware-api.sebvendorportal.com',
  cmsUrl: 'https://ordercloud-cms.azurewebsites.net',
  creditCardIframeUrl: 'https://fts.cardconnect.com/itoke/ajax-tokenizer.html',
  translateBlobUrl:
    'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
  sellerID: 'pPOiukEUHkSGrBmAIjdReQ',
  ssoLink:
    'https://stage-authorize.anytimefitness.com/authorize?response_type=code&client_id=86d70db9-22e6-47ba-a1ab-bbe00c9b6451&redirect_uri=https://selfesteembrands-api-qa.azurewebsites.net/authorize',
  ordercloudEnv: OrdercloudEnv.Production,
  theme: bmTheme,
  instrumentationKey: '419a0c62-a800-4d19-882b-b61007e69cdb',
}
/*

 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
