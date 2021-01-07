import { EnvironmentConfig, OrdercloudEnv } from 'src/app/models/environment.types'
import bmTheme from '../styles/themes/bar-method/theme-config'

export const environment: EnvironmentConfig = {
  hostedApp: true,
  appname: 'The Bar Method',
  clientID: 'CF547B04-1826-427D-9940-824805F0ECA0',
  marketplaceID: 'SEB_TEST',
  baseUrl: 'https://thebarmethod-staging.sebvendorportal.com',
  middlewareUrl: 'https://middleware-api-staging.sebvendorportal.com',
  cmsUrl: 'https://ordercloud-cms-staging.azurewebsites.net',
  creditCardIframeUrl:
    'https://fts-uat.cardconnect.com/itoke/ajax-tokenizer.html',
  translateBlobUrl:
    'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
  sellerID: 'pPOiukEUHkSGrBmAIjdReQ',
  ssoLink: null,
  ordercloudEnv: OrdercloudEnv.Staging,
  theme: bmTheme,
  instrumentationKey: 'ec212601-1d32-4fd4-872f-7f073b50ae7c',
}
