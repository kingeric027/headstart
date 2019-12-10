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
