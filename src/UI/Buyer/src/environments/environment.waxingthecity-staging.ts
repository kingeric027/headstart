import { AppConfig } from './environment.interfaces'
import wtcTheme from '../styles/themes/waxing-the-city/theme-config'
import { OrdercloudEnv } from 'src/app/shopper-context'

export const environment: AppConfig = {
  hostedApp: true,
  appname: 'Waxing The City',
  clientID: '0BF4E739-7C2A-45A9-9A08-2AD44EB75F1D',
  marketplaceID: 'SEB_TEST',
  baseUrl: 'https://waxingthecity-staging.sebvendorportal.com',
  middlewareUrl: 'https://middleware-api-staging.sebvendorportal.com',
  cmsUrl: 'https://ordercloud-cms-staging.azurewebsites.net',
  creditCardIframeUrl:
    'https://fts-uat.cardconnect.com/itoke/ajax-tokenizer.html',
  translateBlobUrl:
    'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
  sellerID: 'pPOiukEUHkSGrBmAIjdReQ',
  ssoLink: null,
  ordercloudEnv: OrdercloudEnv.Staging,
  theme: wtcTheme,
  instrumentationKey: 'ec212601-1d32-4fd4-872f-7f073b50ae7c',
}
/*

 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
