import { EnvironmentConfig, OrdercloudEnv } from 'src/app/models/environment.types'
import wtcTheme from '../styles/themes/waxing-the-city/theme-config'

export const environment: EnvironmentConfig = {
  hostedApp: true,
  appname: 'Waxing The City',
  clientID: '0BF4E739-7C2A-45A9-9A08-2AD44EB75F1D',
  marketplaceID: 'SEB',
  baseUrl: 'https://waxingthecity.sebvendorportal.com',
  middlewareUrl: 'https://middleware-api.sebvendorportal.com',
  cmsUrl: 'https://ordercloud-cms.azurewebsites.net',
  creditCardIframeUrl: 'https://fts.cardconnect.com/itoke/ajax-tokenizer.html',
  translateBlobUrl:
    'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
  sellerID: 'pPOiukEUHkSGrBmAIjdReQ',
  ssoLink: null,
  ordercloudEnv: OrdercloudEnv.Production,
  theme: wtcTheme,
  instrumentationKey: '419a0c62-a800-4d19-882b-b61007e69cdb',
}
