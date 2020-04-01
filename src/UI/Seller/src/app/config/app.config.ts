import { InjectionToken } from '@angular/core';
import { environment } from '../../environments/environment';

export const ocAppConfig: AppConfig = {
  appname: environment.appname,
  marketplaceID: environment.marketplaceID,
  cmsUrl: environment.cmsUrl,
  clientID: environment.clientID,
  middlewareUrl: environment.middlewareUrl,
  orderCloudApiUrl: environment.orderCloudApiUrl,
  orderCloudAuthUrl: environment.orderCloudAuthUrl,
  orderCloudApiVersion: environment.orderCloudApiVersion,
  // sellerName is being hard-coded until this is available to store in OrderCloud
  sellerName: 'SEB Seller',
  scope: [
    // 'AdminAddressReader' is just for reading admin addresses as a seller user on product create/edti
    // Will need to be updated to 'AdminAddressAdmin' when seller address create is implemented
    'AdminAddressReader',
    'MeAddressAdmin',
    'MeAdmin',
    'BuyerUserAdmin',
    'UserGroupAdmin',
    'MeCreditCardAdmin',
    'MeXpAdmin',
    'Shopper',
    'CategoryReader',
    'ProductAdmin',

    // adding this for product editing and creation on the front end
    // this logic may be moved to the backend in the future and this might not be required
    'PriceScheduleAdmin',

    'SupplierReader',
    'SupplierAddressReader',
    'BuyerAdmin',
    'OverrideUnitPrice',
    'OrderAdmin',
    'OverrideTax',
    'OverrideShipping',
    'BuyerImpersonation',
    'AddressAdmin',
    'CategoryAdmin',
    'CatalogAdmin',
    'PromotionAdmin',
    'ApprovalRuleAdmin',
    'CreditCardAdmin',
    'SupplierAdmin',
    'SupplierUserAdmin',
    'SupplierUserGroupAdmin',
    'SupplierAddressAdmin',
    'AdminUserAdmin',
    'ProductFacetAdmin',
    'ProductFacetReader',

    // custom roles used to conditionally display ui
    'MPMeProductAdmin',
    'MPMeProductReader',
    'MPProductAdmin',
    'MPProductReader',
    'MPPromotionAdmin',
    'MPPromotionReader',
    'MPCategoryAdmin',
    'MPCategoryReader',
    'MPOrderAdmin',
    'MPOrderReader',
    'MPShipmentAdmin',
    'MPBuyerAdmin',
    'MPBuyerReader',
    'MPSellerAdmin',
    'MPReportReader',
    'MPSupplierAdmin',
    'MPMeSupplierAdmin',
    'MPMeSupplierAddressAdmin',
    'MPMeSupplierUserAdmin',
    'MPSupplierUserGroupAdmin',
    'MPStoreFrontAdmin',
  ],
};

export const applicationConfiguration = new InjectionToken<AppConfig>('app.config', {
  providedIn: 'root',
  factory: () => ocAppConfig,
});

export interface AppConfig {
  /**
   * A short name for your app. It will be used as a
   * cookie prefix as well as general display throughout the app.
   */
  appname: string;

  /**
   * The identifier for the seller, buyer network or buyer application that
   * will be used for authentication. You can view client ids for apps
   * you own or are a contributor to on the [dashboard](https://developer.ordercloud.io/dashboard)
   */
  clientID: string;
  /**
   * base path to middleware
   */
  middlewareUrl: string;

  cmsUrl: string;

  marketplaceID: string;

  // sellerName is being hard-coded until this is available to store in OrderCloud
  sellerName: string;

  /**
   * An array of security roles that will be requested upon login.
   * These roles allow access to specific endpoints in the OrderCloud.io API.
   * To learn more about these roles and the security profiles that comprise them
   * read [here](https://developer.ordercloud.io/documentation/platform-guides/authentication/security-profiles)
   */

  orderCloudApiUrl: string;
  orderCloudAuthUrl: string;
  orderCloudApiVersion: string;

  scope: string[];
}
