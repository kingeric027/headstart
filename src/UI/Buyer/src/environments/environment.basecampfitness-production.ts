// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.

import { Environment } from './environment.interface'
import bcfTheme from '../styles/themes/basecamp/theme-config'
import { OrdercloudEnv } from 'src/app/shopper-context'

export const environment: Environment = {
  hostedApp: true,
  appname: 'Basecamp Fitness',
  clientID: '0045BB67-84CC-42BD-9FA7-009875628F7C',
  marketplaceID: 'SEB',
  baseUrl: 'https://basecampfitness.sebvendorportal.com',
  middlewareUrl: 'https://api.sebvendorportal.com',
  cmsUrl: 'https://ordercloud-cms.azurewebsites.net',  
  creditCardIframeUrl: 'https://fts.cardconnect.com/itoke/ajax-tokenizer.html',
  translateBlobUrl:
    'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
  sellerID: 'pPOiukEUHkSGrBmAIjdReQ',
  ssoLink: null,
  ordercloudEnv: OrdercloudEnv.Production,
  theme: bcfTheme,
  instrumentationKey: '22575a56-7540-47c7-9a91-3cf5316531a3',
}
/*

 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
