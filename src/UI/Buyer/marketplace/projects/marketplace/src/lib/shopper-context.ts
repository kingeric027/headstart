import {
  LineItem,
  Order,
  ListLineItem,
  ListPayment,
  ListPromotion,
  OrderApproval,
  Shipment,
  ShipmentItem,
  BuyerProduct,
  Supplier,
  Address,
  ListBuyerProduct,
  ListAddress,
  ListBuyerCreditCard,
  BuyerCreditCard,
  User,
  UserGroupAssignment,
  ApprovalRule,
} from '@ordercloud/angular-sdk';
import { ProductXp, BuyerAddressXP, MarketplaceAddressBuyer, TaxCertificate } from 'marketplace-javascript-sdk';

export * from '@ordercloud/angular-sdk';
export * from './services/shopper-context/shopper-context.service';
export * from '../../src/lib/services/ordercloud-sandbox/ordercloud-sandbox.models';
export {
  OrderCloudIntegrationsCreditCardToken,
  OrderCloudIntegrationsCreditCardPayment,
} from 'marketplace-javascript-sdk';

export interface LineItemGroupSupplier {
  supplier: Supplier;
  shipFrom: Address;
}

export interface SupplierFilters {
  supplierID?: string;
  page?: number;
  sortBy?: string;
  activeFilters?: any;
  search?: string;
}

export interface ShippingRate {
  Id: string;
  AccountName: string;
  Carrier: string;
  Currency: string;
  DeliveryDate: Date;
  DeliveryDays: number;
  CarrierQuoteId: string;
  Service: string;
  TotalCost: number;
}

export interface MarketplaceOrder extends Order<OrderXp, any, any> {}
export interface OrderXp {
  AvalaraTaxTransactionCode?: string;
  OrderType?: OrderType;
  QuoteOrderInfo?: QuoteOrderInfo;
}

export enum OrderType {
  Standard = 'Standard',
  Quote = 'Quote',
}

export interface QuoteOrderInfo {
  FirstName: string;
  LastName: string;
  Phone: string;
  Email: string;
  Comments?: string;
}

export interface ProductFilters {
  page?: number;
  sortBy?: string;
  search?: string;
  showOnlyFavorites?: boolean;
  categoryID?: string;
  activeFacets?: any;
}

export interface OrderFilters {
  page?: number;
  sortBy?: string;
  search?: string;
  showOnlyFavorites?: boolean;
  status?: OrderStatus;
  /**
   * mm-dd-yyyy
   */
  fromDate?: string;
  /**
   * mm-dd-yyyy
   */
  toDate?: string;
  location?: string;
}

export enum OrderAddressType {
  Billing = 'Billing',
  Shipping = 'Shipping',
}

export enum OrderStatus {
  AllSubmitted = '!Unsubmitted',
  AwaitingApproval = 'AwaitingApproval',
  ChangesRequested = 'ChangesRequested',
  Open = 'Open',
  Completed = 'Completed',
  Canceled = 'Canceled',
}

export enum OrderViewContext {
  MyOrders = 'MyOrders',
  Approve = 'Approve',
  Location = 'Location',
}

export interface CreditCard {
  CardholderName: string;
  CardNumber: string;
  ExpirationDate: string;
  SecurityCode: string;
  ID?: string;
}

export interface OrderReorderResponse {
  ValidLi: Array<LineItem>;
  InvalidLi: Array<LineItem>;
}

export interface OrderDetails {
  order: MarketplaceOrder;
  lineItems: ListLineItem;
  promotions: ListPromotion;
  payments: ListPayment;
  approvals: OrderApproval[];
}

export interface ShipmentWithItems extends Shipment {
  ShipmentItems: ShipmentItemWithLineItem[];
}

export interface ShipmentItemWithLineItem extends ShipmentItem {
  LineItem: LineItem;
}

/**
 * LineItem with the full product details. Currently used in the cart page only.
 */
export interface LineItemWithProduct extends LineItem {
  Product?: BuyerProduct;
}

/**
 * List of lineItems with full product details. Currently used in the cart page only.
 */
export interface ListLineItemWithProduct extends ListLineItem {
  Items: Array<LineItemWithProduct>;
}

export class AppConfig {
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
   * The identifier for the marketplace.
   */
  marketplaceID: string;
  /**
   * If set to true users can browse and submit orders without profiling themselves. This requires
   * additional set up in the dashboard. Click here to
   * [learn more](https://developer.ordercloud.io/documentation/platform-guides/authentication/anonymous-shopping)
   */
  anonymousShoppingEnabled: boolean;
  cardConnectMerchantID: string;
  baseUrl: string;
  /**
   * base path to middleware
   */

  orderCloudApiUrl: string;
  orderCloudAuthUrl: string;
  orderCloudApiVersion: string;
  avalaraCompanyId: number;
  middlewareUrl: string;
  /**
   * base path to CMS resources
   */
  cmsUrl: string;
  /**
   *  TODO - Link to identity provider's authorization server. this field should probably be SEB-specific.
   */
  ssoLink: string;
  /**
   * An array of security roles that will be requested upon login.
   * These roles allow access to specific endpoints in the OrderCloud.io API.
   * To learn more about these roles and the security profiles that comprise them
   * read [here](https://developer.ordercloud.io/documentation/platform-guides/authentication/security-profiles)
   */
  scope: string[];
}

export interface DecodedOCToken {
  /**
   * the ordercloud username
   */
  usr: string;

  /**
   * the client id used when making token request
   */
  cid: string;

  /**
   * helpful for identifying user types in an app
   * that may have both types
   */
  usrtype: 'admin' | 'buyer';

  /**
   * list of security profile roles that this user
   * has access to, read more about security profile roles
   * [here](https://developer.ordercloud.io/documentation/platform-guides/authentication/security-profiles)
   */
  role: string[]; // TODO: add security profile roles to the sdk

  /**
   * the issuer of the token - should always be https://auth.ordercloud.io
   */
  iss: string;

  /**
   * the audience - who should be consuming this token
   * this should always be https://api.ordercloud.io (the ordercloud api)
   */
  aud: string;

  /**
   * expiration of the token (in seconds) since the
   * UNIX epoch (January 1, 1970 00:00:00 UTC)
   */
  exp: number;

  /**
   * point at which token was issued (in seconds) since the
   * UNIX epoch (January 1, 1970 00:00:00 UTC)
   */
  nbf: number;

  /**
   * the order id assigned to the anonymous user,
   * this value will *only* exist for anonymous users
   */
  orderid?: string;
}

export interface BuyerLocationWithCert {
  location?: MarketplaceAddressBuyer;
  certificate?: TaxCertificate;
}

// Product Model
// a corresponding model in the C# product
export type ListMarketplaceMeProduct = ListBuyerProduct<ProductXp>;

export type MarketplaceMeProduct = BuyerProduct<ProductXp>;

export type ListMarketplaceAddressBuyer = ListAddress<BuyerAddressXP>;

export type ListMarketplaceBuyerCreditCard = ListBuyerCreditCard<CreditCardXP>;

export type MarketplaceBuyerCreditCard = BuyerCreditCard<CreditCardXP>;

export interface CreditCardXP {
  CCBillingAddress: Address;
}
