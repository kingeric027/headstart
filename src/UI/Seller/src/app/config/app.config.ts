import { InjectionToken } from '@angular/core';
import { environment } from '../../environments/environment';

export const ocAppConfig: AppConfig = {
  appname: 'OrderCloud Admin',
  clientID: environment.clientID,
  middlewareUrl: environment.middlewareUrl,
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
    'SupplierAddressAdmin',
    'AdminUserAdmin',

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
    'MPSupplierAdmin',
    'MPMeSupplierAdmin',
    'MPMeSupplierAddressAdmin',
    'MPMeSupplierUserAdmin',
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

  /**
   * An array of security roles that will be requested upon login.
   * These roles allow access to specific endpoints in the OrderCloud.io API.
   * To learn more about these roles and the security profiles that comprise them
   * read [here](https://developer.ordercloud.io/documentation/platform-guides/authentication/security-profiles)
   */
  scope: string[];
}
