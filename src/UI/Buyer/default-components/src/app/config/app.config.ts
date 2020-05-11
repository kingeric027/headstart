import { AppConfig } from 'marketplace';
import { environment } from 'src/environments/environment';

export const ocAppConfig: AppConfig = {
  appname: environment.appname,
  clientID: environment.clientID,
  marketplaceID: environment.marketplaceID,
  baseUrl: environment.baseUrl,
  middlewareUrl: environment.middlewareUrl,
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
    'MPNeedsApproval',
    'MPViewAllLocationOrders',
    'MPOrderApprover',
    'ApprovalRuleAdmin',
    'Shopper',
    'BuyerUserAdmin',
    'BuyerReader',
    'PasswordReset',
    'SupplierReader',
    'SupplierAddressReader',
  ],
};
