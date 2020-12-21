import { EnvironmentConfig } from './environment.interfaces'
import afTheme from '../styles/themes/anytime-fitness/theme-config'
import { OrdercloudEnv } from 'src/app/shopper-context'

export const environment: EnvironmentConfig = {
  hostedApp: true,
  appname: 'Anytime Fitness',
  clientID: 'F18AE28D-FFA4-4A5A-9C69-A1FBC71DCD3D',
  marketplaceID: 'SEB',
  baseUrl: 'https://anytimefitness.sebvendorportal.com',
  middlewareUrl: 'https://middleware-api.sebvendorportal.com',
  cmsUrl: 'https://ordercloud-cms.azurewebsites.net',
  creditCardIframeUrl: 'https://fts.cardconnect.com/itoke/ajax-tokenizer.html',
  translateBlobUrl:
    'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
  sellerID: 'pPOiukEUHkSGrBmAIjdReQ',
  ssoLink: null,
  ordercloudEnv: OrdercloudEnv.Production,
  theme: afTheme,
  instrumentationKey: '419a0c62-a800-4d19-882b-b61007e69cdb',
}
