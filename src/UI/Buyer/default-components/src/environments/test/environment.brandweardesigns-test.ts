import { OrdercloudEnv } from 'marketplace';
import { Environment } from '../env.interface';
import theme from '../../styles/themes/go2partners/theme-config';

export const environment: Environment = {
    hostedApp: true,
    appname: 'BRANDWEAR Designs',
    clientID: '2F33BE12-D914-419C-B3D0-41AEFB72BE93',
    marketplaceID: 'BW',
    baseUrl: 'https://brandweardesigns-buyer-ui-test.azurewebsites.net/',
    middlewareUrl: 'https://brandweardesigns-middleware-test.azurewebsites.net',
    translateBlobUrl: 'https://stbrandweartest.blob.core.windows.net/ngx-translate/i18n/',
    sellerID: 'BRANDWEARDESIGNS_TEST',
    ssoLink: null,
    ordercloudEnv: OrdercloudEnv.Sandbox,
    theme,
};