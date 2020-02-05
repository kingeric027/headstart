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
    'SupplierReader',
    // TODO - remove. In the platform, to create a payment with a saved credit card,
    // you need both access to the credit card and OrderAdmin. Don't go live with OrderAdmin
    // still on this list haha. 
    'OrderAdmin'   
  ]
};
