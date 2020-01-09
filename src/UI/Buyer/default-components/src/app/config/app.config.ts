import { AppConfig } from 'marketplace';

export const ocAppConfig: AppConfig = {
  appname: 'OrderCloud',
  anonymousShoppingEnabled: false,
  clientID: '26B45BD1-5D02-489D-A60E-3B37C852606C',
  marketplaceID: 'SEB',
  baseUrl: 'http://localhost:33333',
  middlewareUrl: 'http://localhost:51100',
  cmsUrl:
    'https://s3.dualstack.us-east-1.amazonaws.com/staticcintas.eretailing.com/images/product',
  ssoLink: 'https://stage-authorize.anytimefitness.com/authorize?response_type=code&client_id=86d70db9-22e6-47ba-a1ab-bbe00c9b6451&redirect_uri=https://selfesteembrands-api-qa.azurewebsites.net/authorize',
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
