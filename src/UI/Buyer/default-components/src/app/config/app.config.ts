import { AppConfig } from 'marketplace';
import { environment } from 'src/environments/environment';

export const ocAppConfig: AppConfig = {
  appname: environment.appname,
  clientID:  environment.clientID,
  marketplaceID: environment.marketplaceID,
  baseUrl: environment.baseUrl,
  middlewareUrl: environment.middlewareUrl,
  cmsUrl: environment.cmsUrl,
  ssoLink: environment.ssoLink,
  anonymousShoppingEnabled: false,
  cardConnectMerchantID: '840000000052',
  scope: [
    'MeAddressAdmin',
    'MeAdmin',
    'MeCreditCardAdmin',
    'MeXpAdmin',
    'Shopper', 
    'BuyerReader',
    'PasswordReset',
    'SupplierReader'
  ]
};
