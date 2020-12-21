import { EnvironmentConfig } from './environment.interfaces'
import theme from '../styles/themes/fastsigns/theme-config'
import { OrdercloudEnv } from 'src/app/shopper-context'

export const environment: EnvironmentConfig = {
  hostedApp: true,
  appname: 'FASTSIGNS',
  clientID: '3B7CD2F7-36D8-4DC4-9616-0CB1C86C9FB3',
  marketplaceID: 'FS_TEST',
  baseUrl: 'https://fastsigns-buyer-ui-test.azurewebsites.net/',
  cmsUrl: 'https://ordercloud-cms-test.azurewebsites.net',
  middlewareUrl: 'https://fastsigns-middleware-test.azurewebsites.net',
  creditCardIframeUrl:
    'https://fts-uat.cardconnect.com/itoke/ajax-tokenizer.html',
  instrumentationKey: '4133d3cf-ae7a-43d1-a16b-172aee7cff6f',
  translateBlobUrl:
    'https://stfastsignstest.blob.core.windows.net/ngx-translate/i18n/',
  sellerID: 'FASTSIGNS_TEST',
  ssoLink: null,
  ordercloudEnv: OrdercloudEnv.Sandbox,
  theme,
}
