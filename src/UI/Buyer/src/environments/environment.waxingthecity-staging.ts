import { EnvironmentConfig } from './environment.interfaces'
import wtcTheme from '../styles/themes/waxing-the-city/theme-config'
import { OrdercloudEnv } from 'src/app/shopper-context'

export const environment: EnvironmentConfig = {
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
