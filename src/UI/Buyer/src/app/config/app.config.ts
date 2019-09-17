import { InjectionToken } from '@angular/core';
import { PaymentMethod } from 'src/app/shared/models/payment-method.enum';
import { environment } from '../../environments/environment';

export const ocAppConfig: AppConfig = {
  appname: 'OrderCloud',
  clientID: environment.clientID,
  anonymousShoppingEnabled: false,
  middlewareUrl: environment.middlewareUrl,
  cmsUrl: environment.cmsUrl,
  scope: ['MeAddressAdmin', 'MeAdmin', 'MeCreditCardAdmin', 'MeXpAdmin', 'Shopper', 'BuyerReader', 'PasswordReset'],
  availablePaymentMethods: [PaymentMethod.PurchaseOrder, PaymentMethod.SpendingAccount],
};

export const applicationConfiguration = new InjectionToken<AppConfig>('app.config', { providedIn: 'root', factory: () => ocAppConfig });

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
   * If set to true users can browse and submit orders without profiling themselves. This requires
   * additional set up in the dashboard. Click here to
   * [learn more](https://developer.ordercloud.io/documentation/platform-guides/authentication/anonymous-shopping)
   */
  anonymousShoppingEnabled: boolean;

  /**
   * base path to middleware
   */
  middlewareUrl: string;

  /**
   * base path to CMS resources
   */
  cmsUrl: string;
  /**
   * An array of security roles that will be requested upon login.
   * These roles allow access to specific endpoints in the OrderCloud.io API.
   * To learn more about these roles and the security profiles that comprise them
   * read [here](https://developer.ordercloud.io/documentation/platform-guides/authentication/security-profiles)
   */
  scope: string[];

  /**
   * this defines the types of payment methods a user can select
   * from to complete payment on an order
   */
  availablePaymentMethods: PaymentMethod[];
}
