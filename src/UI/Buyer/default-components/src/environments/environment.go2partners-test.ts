import { OrdercloudEnv } from 'marketplace';
import { Environment } from './environment.interface';
import theme from '../styles/themes/go2partners/theme-config';

export const environment: Environment = {
    hostedApp: true,
    appname: 'GO2 Partners',
    clientID: 'B1FEB16F-9E3E-4534-88FE-F3AE29941986',
    marketplaceID: 'GO2_TEST',
    baseUrl: 'https://go2partners-buyer-ui-test.azurewebsites.net/',
    middlewareUrl: 'https://go2partners-middleware-test.azurewebsites.net',
    creditCardIframeUrl: 'https://fts-uat.cardconnect.com/itoke/ajax-tokenizer.html',
    instrumentationKey: '',
    translateBlobUrl: 'https://stgo2partnerstest.blob.core.windows.net/ngx-translate/i18n/',
    sellerID: 'GO2PARTNERS_TEST',
    ssoLink: null,
    ordercloudEnv: OrdercloudEnv.Sandbox,
    theme,
};