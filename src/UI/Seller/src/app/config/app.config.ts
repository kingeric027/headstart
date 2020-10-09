import { InjectionToken } from '@angular/core';
import { environment } from '../../environments/environment';
import { ApiRole } from '@ordercloud/angular-sdk';

export const ocAppConfig: AppConfig = {
  appname: environment.appname,
  clientID: environment.clientID,
  sellerID: environment.sellerID,
  middlewareUrl: environment.middlewareUrl,
  orderCloudApiUrl: environment.orderCloudApiUrl,
  orderCloudApiVersion: environment.orderCloudApiVersion,
  translateBlobUrl: environment.translateBlobUrl,
  blobStorageUrl: environment.blobStorageUrl,
  buyerConfigs: environment.buyerConfigs,
  // sellerName is being hard-coded until this is available to store in OrderCloud
  sellerName: 'SEB Seller',
  scope: [
    // 'AdminAddressReader' is just for reading admin addresses as a seller user on product create/edti
    // Will need to be updated to 'AdminAddressAdmin' when seller address create is implemented
    'ApiClientAdmin',
    'ApiClientReader',
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
    'SupplierAddressReader',
    'AdminUserAdmin',
    'ProductFacetAdmin',
    'ProductFacetReader',
    'ShipmentAdmin',

    // custom cms roles
    'AssetAdmin',
    'DocumentAdmin',
    'SchemaAdmin',

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
    'MPReportAdmin',
    'MPSupplierAdmin',
    'MPMeSupplierAdmin',
    'MPMeSupplierAddressAdmin',
    'MPMeSupplierUserAdmin',
    'MPSupplierUserGroupAdmin',
    'MPStorefrontAdmin',
  ] as ApiRole[],
  impersonatingBuyerScope: [
    'MeAddressAdmin',
    'AddressAdmin', // Only for location owners
    'MeAdmin',
    'MeCreditCardAdmin',
    'MeXpAdmin',
    'UserGroupAdmin',
    'ApprovalRuleAdmin',
    'BuyerUserAdmin',
    'Shopper',
    'BuyerReader',
    'PasswordReset',
    'SupplierReader',
    'SupplierAddressReader',
  ] as ApiRole[],
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
  sellerID: string;
  clientID: string;
  /**
   * base path to middleware
   */
  middlewareUrl: string;

  translateBlobUrl: string;
  blobStorageUrl: string;

  // sellerName is being hard-coded until this is available to store in OrderCloud
  sellerName: string;

  //  buyer url and client ID are needed for impersonating buyers
  buyerConfigs: any;

  /**
   * An array of security roles that will be requested upon login.
   * These roles allow access to specific endpoints in the OrderCloud.io API.
   * To learn more about these roles and the security profiles that comprise them
   * read [here](https://developer.ordercloud.io/documentation/platform-guides/authentication/security-profiles)
   */

  orderCloudApiUrl: string;
  orderCloudApiVersion: string;

  scope: ApiRole[];
  impersonatingBuyerScope: ApiRole[];
}
