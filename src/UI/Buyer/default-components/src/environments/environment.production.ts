import { OrdercloudEnv } from 'marketplace';
import { Environment } from './env.interface';
import wtcTheme from 'src/styles/themes/waxing-the-city/theme-config';

export const environment: Environment = {
  hostedApp: true,
  appname: 'Self Esteem Brands',
  clientID: '0BF4E739-7C2A-45A9-9A08-2AD44EB75F1D',
  marketplaceID: 'SEB',
  baseUrl: 'http://marketplace-buyer-ui.azurewebsites.net/',
  middlewareUrl: 'https://marketplace-middleware.azurewebsites.net',
  translateBlobUrl: 'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
  ssoLink:
    'https://stage-authorize.anytimefitness.com/authorize?response_type=code&client_id=86d70db9-22e6-47ba-a1ab-bbe00c9b6451&redirect_uri=https://selfesteembrands-api-qa.azurewebsites.net/authorize',
  ordercloudEnv: OrdercloudEnv.Production,
  theme: wtcTheme,
};
