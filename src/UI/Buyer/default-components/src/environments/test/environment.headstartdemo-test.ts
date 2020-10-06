import { OrdercloudEnv } from 'marketplace';
import { Environment } from '../env.interface';
import theme from '../../styles/themes/headstartdemo/theme-config';

export const environment: Environment = {
    hostedApp: true,
    appname: 'Headstart Demo',
    clientID: 'A482C18B-527B-4BA1-A2E9-0E7C65C2E39F',
    marketplaceID: 'DEMO',
    baseUrl: 'https://headstartdemo-buyer-ui-test.azurewebsites.net/',
    middlewareUrl: 'https://headstartdemo-middleware-test.azurewebsites.net',
    translateBlobUrl: 'https://stfour51demotest.blob.core.windows.net/ngx-translate2/i18n/',
    sellerID: 'Headstart_Demo_Test',
    ssoLink: null,
    ordercloudEnv: OrdercloudEnv.Sandbox,
    theme,
};