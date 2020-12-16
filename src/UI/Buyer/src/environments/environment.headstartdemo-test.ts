import { Environment } from './environment.interface'
import theme from '../styles/themes/headstartdemo/theme-config'
import { OrdercloudEnv } from 'src/app/shopper-context'

export const environment: Environment = {
  hostedApp: true,
  appname: 'Headstart Demo',
  clientID: 'A482C18B-527B-4BA1-A2E9-0E7C65C2E39F',
  marketplaceID: 'DEMO_TEST',
  baseUrl: 'https://headstartdemo-buyer-ui-test.azurewebsites.net/',
  middlewareUrl: 'https://headstartdemo-middleware-test.azurewebsites.net',
  cmsUrl: 'https://ordercloud-cms-test.azurewebsites.net', 
  creditCardIframeUrl:
    'https://fts-uat.cardconnect.com/itoke/ajax-tokenizer.html',
  instrumentationKey: '731925bd-018c-447e-8ba5-06b432d708c8',
  translateBlobUrl:
    'https://stfour51demotest.blob.core.windows.net/ngx-translate2/i18n/',
  sellerID: 'Headstart_Demo_Test',
  ssoLink: null,
  ordercloudEnv: OrdercloudEnv.Sandbox,
  theme,
}
