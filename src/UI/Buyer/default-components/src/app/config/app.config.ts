import { AppConfig } from 'marketplace';
import { environment } from 'src/environments/environment';
import { ApiRole } from '../../../../marketplace/node_modules/ordercloud-javascript-sdk/dist';

export const ocAppConfig: AppConfig = {
  appname: environment.appname,
  clientID: environment.clientID,
  marketplaceID: environment.marketplaceID,
  baseUrl: environment.baseUrl,
  middlewareUrl: environment.middlewareUrl,
  ssoLink: environment.ssoLink,
  ordercloudEnv: environment.ordercloudEnv,
  anonymousShoppingEnabled: false,
  cardConnectMerchantID: '840000000052', // TODO - look for somewhere else to put this.
  avalaraCompanyId: 280411,
  scope: [
    'MeAddressAdmin',
    'AddressAdmin', // Only for location owners
    'MeAdmin',
    'MeCreditCardAdmin',
    'MeXpAdmin',
    'UserGroupAdmin',
    'ApprovalRuleAdmin',
    'Shopper',
    'BuyerUserAdmin',
    'BuyerReader',
    'PasswordReset',
    'SupplierReader',
    'SupplierAddressReader',
    'MPApprovalRuleAdmin',

    // location roles, will appear on jwt if a user
    // has this role for any location
    'MPLocationPermissionAdmin',
    'MPLocationOrderApprover',
    'MPLocationNeedsApproval',
    'MPLocationViewAllOrders',
    'MPLocationCreditCardAdmin',
    'MPLocationAddressAdmin',
    'MPLocationResaleCertAdmin',
  ] as ApiRole[],
};
