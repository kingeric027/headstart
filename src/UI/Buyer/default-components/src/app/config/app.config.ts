import { AppConfig } from 'marketplace';

export const ocAppConfig: AppConfig = {
  appname: 'OrderCloud',
  anonymousShoppingEnabled: false,
  clientID: '78F16865-A4C3-4D28-832D-A0371A93F1EA',
  marketplaceID: 'SEB',
  baseUrl: 'http://localhost:33333',
  middlewareUrl: 'https://marketplace-api-qa.azurewebsites.net',
  cmsUrl:
    'https://s3.dualstack.us-east-1.amazonaws.com/staticcintas.eretailing.com/images/product',
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
