import { OrdercloudEnv } from 'marketplace'
import { Environment } from './environment.interface'
import theme from '../styles/themes/brandweardesigns/theme-config'

export const environment: Environment = {
  hostedApp: true,
  appname: 'BRANDWEAR Designs',
  clientID: '2F33BE12-D914-419C-B3D0-41AEFB72BE93',
  marketplaceID: 'BW_TEST',
  baseUrl: 'https://brandweardesigns-buyer-ui-test.azurewebsites.net/',
  middlewareUrl: 'https://brandweardesigns-middleware-test.azurewebsites.net',
  creditCardIframeUrl:
    'https://fts-uat.cardconnect.com/itoke/ajax-tokenizer.html',
  instrumentationKey: '0b1879d3-d708-49cb-bd48-7363be74a837',
  translateBlobUrl:
    'https://stbrandweartest.blob.core.windows.net/ngx-translate/i18n/',
  sellerID: 'BRANDWEARDESIGNS_TEST',
  ssoLink: null,
  ordercloudEnv: OrdercloudEnv.Sandbox,
  theme,
}
