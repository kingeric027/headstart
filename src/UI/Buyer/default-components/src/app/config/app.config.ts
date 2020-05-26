import { AppConfig } from 'marketplace';
import { environment } from 'src/environments/environment';

export const ocAppConfig: AppConfig = {
  appname: environment.appname,
  clientID: environment.clientID,
  marketplaceID: environment.marketplaceID,
  baseUrl: environment.baseUrl,
  middlewareUrl: environment.middlewareUrl,
  ocMiddlewareUrl: environment.ocMiddlewareUrl,
  cmsUrl: environment.cmsUrl,
  ssoLink: environment.ssoLink,
  orderCloudApiUrl: environment.orderCloudApiUrl,
  orderCloudAuthUrl: environment.orderCloudApiVersion,
  orderCloudApiVersion: environment.orderCloudApiVersion,
  anonymousShoppingEnabled: false,
  cardConnectMerchantID: '840000000052', // TODO - look for somewhere else to put this.
  avalaraCompanyId: 280411,
  scope: [
    'MeAddressAdmin',
    'AddressAdmin', // Only for location owners
    'MeAdmin',
    'MeCreditCardAdmin',
    'MeXpAdmin',
    'MPApprovalRuleAdmin',
    'UserGroupAdmin',
    'ApprovalRuleAdmin',
    'Shopper',
    'BuyerUserAdmin',
    'BuyerReader',
    'PasswordReset',
    'SupplierReader',
    'SupplierAddressReader',

    // location roles, will appear on jwt if a user
    // has this role for any location
    'MPLocationPermissionAdmin',
    'MPLocationOrderApprover',
    'MPLocationNeedsApproval',
    'MPLocationViewAllOrders',
    'MPLocationCreditCardAdmin',
    'MPLocationAddressAdmin',
    'MPLocationResaleCertAdmin',
  ],
};
