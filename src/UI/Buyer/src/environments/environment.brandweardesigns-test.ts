import { EnvironmentConfig } from './environment.interfaces'
import theme from '../styles/themes/brandweardesigns/theme-config'
import { OrdercloudEnv } from 'src/app/shopper-context'

export const environment: EnvironmentConfig = {
  hostedApp: true,
  appname: 'BRANDWEAR Designs',
  clientID: '2F33BE12-D914-419C-B3D0-41AEFB72BE93',
  marketplaceID: 'BW_TEST',
  baseUrl: 'https://brandweardesigns-buyer-ui-test.azurewebsites.net/',
  middlewareUrl: 'https://brandweardesigns-middleware-test.azurewebsites.net',
  cmsUrl: 'https://ordercloud-cms-test.azurewebsites.net',
  creditCardIframeUrl:
    'https://fts-uat.cardconnect.com/itoke/ajax-tokenizer.html',
  instrumentationKey: '4133d3cf-ae7a-43d1-a16b-172aee7cff6f',
  translateBlobUrl:
    'https://stbrandweartest.blob.core.windows.net/ngx-translate/i18n/',
  sellerID: 'BRANDWEARDESIGNS_TEST',
  ssoLink: null,
  ordercloudEnv: OrdercloudEnv.Sandbox,
  theme,
}
