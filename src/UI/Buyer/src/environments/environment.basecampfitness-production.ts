import { EnvironmentConfig } from './environment.interfaces'
import bcfTheme from '../styles/themes/basecamp/theme-config'
import { OrdercloudEnv } from 'src/app/shopper-context'

export const environment: EnvironmentConfig = {
  hostedApp: true,
  appname: 'Basecamp Fitness',
  clientID: '0045BB67-84CC-42BD-9FA7-009875628F7C',
  marketplaceID: 'SEB',
  baseUrl: 'https://basecampfitness.sebvendorportal.com',
  middlewareUrl: 'https://middleware-api.sebvendorportal.com',
  cmsUrl: 'https://ordercloud-cms.azurewebsites.net',
  creditCardIframeUrl: 'https://fts.cardconnect.com/itoke/ajax-tokenizer.html',
  translateBlobUrl:
    'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
  sellerID: 'pPOiukEUHkSGrBmAIjdReQ',
  ssoLink: null,
  ordercloudEnv: OrdercloudEnv.Production,
  theme: bcfTheme,
  instrumentationKey: '419a0c62-a800-4d19-882b-b61007e69cdb',
}
