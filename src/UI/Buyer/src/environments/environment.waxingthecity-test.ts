// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --configuration=qa` replaces `environment.ts` with `environment.qa.ts`.
// The list of file replacements can be found in `angular.json`.

import { Environment } from './environment.interface'
import wtcTheme from '../styles/themes/waxing-the-city/theme-config'
import { OrdercloudEnv } from 'src/app/shopper-context'

export const environment: Environment = {
  hostedApp: true,
  appname: 'Marketplace Test',
  clientID: 'A5231DF1-2B00-4002-AB40-738A9E2CEC4B',
  marketplaceID: 'SEB_TEST',
  baseUrl: 'https://marketplace-buyer-ui-test.azurewebsites.net/',
  middlewareUrl: 'https://seb-middleware-test.azurewebsites.net',
  cmsUrl: 'https://ordercloud-cms-test.azurewebsites.net',  
  creditCardIframeUrl:
    'https://fts-uat.cardconnect.com/itoke/ajax-tokenizer.html',
  translateBlobUrl:
    'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
  sellerID: 'rQYR6T6ZTEqVrgv8x_ei0g',
  ssoLink:
    'https://stage-authorize.anytimefitness.com/authorize?response_type=code&client_id=86d70db9-22e6-47ba-a1ab-bbe00c9b6451&redirect_uri=https://selfesteembrands-api-qa.azurewebsites.net/authorize',
  ordercloudEnv: OrdercloudEnv.Staging,
  theme: wtcTheme,
  instrumentationKey: '3a0b0eb7-9a02-4f97-b75d-c4811aec975e',
}

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
