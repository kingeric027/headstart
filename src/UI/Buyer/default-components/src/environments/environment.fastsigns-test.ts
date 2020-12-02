import { OrdercloudEnv } from 'marketplace'
import { Environment } from './environment.interface'
import theme from '../styles/themes/fastsigns/theme-config'

export const environment: Environment = {
  hostedApp: true,
  appname: 'FASTSIGNS',
  clientID: '3B7CD2F7-36D8-4DC4-9616-0CB1C86C9FB3',
  marketplaceID: 'FS_TEST',
  baseUrl: 'https://fastsigns-buyer-ui-test.azurewebsites.net/',
  middlewareUrl: 'https://fastsigns-middleware-test.azurewebsites.net',
  creditCardIframeUrl:
    'https://fts-uat.cardconnect.com/itoke/ajax-tokenizer.html',
  instrumentationKey: 'e68a2c8b-3684-4ba9-bbca-5cc9795afd65',
  translateBlobUrl:
    'https://stfastsignstest.blob.core.windows.net/ngx-translate/i18n/',
  sellerID: 'FASTSIGNS_TEST',
  ssoLink: null,
  ordercloudEnv: OrdercloudEnv.Sandbox,
  theme,
}
