import { EnvironmentConfig, OrdercloudEnv } from 'src/app/models/environment.types'
import afTheme from '../styles/themes/anytime-fitness/theme-config'

export const environment: EnvironmentConfig = {
  hostedApp: true,
  appname: 'Anytime Fitness',
  clientID: 'F18AE28D-FFA4-4A5A-9C69-A1FBC71DCD3D',
  marketplaceID: 'SEB_TEST',
  baseUrl: 'https://anytimefitness-staging.sebvendorportal.com',
  middlewareUrl: 'https://middleware-api-staging.sebvendorportal.com',
  cmsUrl: 'https://ordercloud-cms-staging.azurewebsites.net',
  creditCardIframeUrl:
    'https://fts-uat.cardconnect.com/itoke/ajax-tokenizer.html',
  translateBlobUrl:
    'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
  sellerID: 'pPOiukEUHkSGrBmAIjdReQ',
  ssoLink:
    'https://stage-authorize.anytimefitness.com/authorize?response_type=code&client_id=7f9257b2-8a27-4527-9efa-225f4cb172da&redirect_uri=https://seb-four51-integration-stage.azurewebsites.net/api/anytime/authorize',
  ordercloudEnv: OrdercloudEnv.Staging,
  theme: afTheme,
  instrumentationKey: 'ec212601-1d32-4fd4-872f-7f073b50ae7c',
}
