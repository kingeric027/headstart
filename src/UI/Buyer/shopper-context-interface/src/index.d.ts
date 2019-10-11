import { LineItem, MeUser, Order, ListLineItem, AccessToken, PasswordReset, User, Address, ListPayment, BuyerCreditCard, OcMeService, Payment, ListPromotion, OrderApproval, Promotion } from '@ordercloud/angular-sdk';
import { Observable, Subject } from 'rxjs';

export interface IShopperContext {
  router: IRouter;
  currentUser: ICurrentUser;
  currentOrder: ICurrentOrder;
  productFilters: IProductFilters;
  authentication: IAuthentication;
  orderHistory: IOrderHistory;
  creditCards: ICreditCards;
  myResources: OcMeService; // TODO - create our own, more limited interface here. Me.Patch(), for example, should not be allowed since it should always go through the current user service.
  appSettings: AppConfig; // TODO - should this come from custom-components repo somehow? Or be configured in admin and persisted in db?
}

export interface ICreditCards {
  CreateSavedCard(card: AuthNetCreditCard): Promise<CreateCardResponse>;
  DeleteSavedCard(cardID: string): Promise<void>;
}

export interface IOrderHistory {
  activeOrderID: string;
  approveOrder(orderID?: string, Comments?: string,  AllowResubmit?: boolean): Promise<Order>;
  declineOrder(orderID?: string, Comments?: string, AllowResubmit?: boolean): Promise<Order>;
  validateReorder(orderID?: string): Promise<OrderReorderResponse>;
  getOrderDetails(orderID?: string): Promise<OrderDetails>;
}

export interface IRouter {
  getActiveUrl(): string;
  onUrlChange(callback: (path: string) => void): void;
  toProductDetails(productID: string): void;
  toProductList(options?: ProductFilters): void;
  toCheckout(): void;
  toHome(): void;
  toCart(): void;
  toLogin(): void;
  toRegister(): void;
  toForgotPassword(): void;
  toMyProfile(): void;
  toMyAddresses(): void;
  toMyPaymentMethods(): void;
  toMyOrders(): void;
  toOrdersToApprove(): void;
  toOrderDetails(orderID: string): void;
  toChangePassword(): void;
}

export interface ICurrentUser {
  favoriteProductIDs: string[];
  favoriteOrderIDs: string[];
  isAnonymous: boolean;
  get(): MeUser;
  patch(user: MeUser): Promise<MeUser>;
  onUserChange(callback: (user: User) => void): void; // TODO - replace all these onChange functions with real Observables. More powerful
  onIsAnonymousChange(callback: (isAnonymous: boolean) => void): void;
  onFavoriteProductsChange(callback: (productIDs: string[]) => void): void;
  setIsFavoriteProduct(isFav: boolean, productID: string): void;
  onFavoriteOrdersChange(callback: (orderIDs: string[]) => void): void;
  setIsFavoriteOrder(isFav: boolean, orderID: string): void;
}

export interface ICurrentOrder {
  addToCartSubject: Subject<LineItem>;
  get(): Order;
  patch(order: Order): Promise<Order>; 
  getLineItems(): ListLineItem;
  submit(): Promise<void>;

  addToCart(lineItem: LineItem): Promise<LineItem>;
  addManyToCart(lineItem: LineItem[]): Promise<LineItem[]>;
  setQuantityInCart(lineItemID: string, newQuantity: number): Promise<LineItem>;
  removeFromCart(lineItemID: string): Promise<void>;
  emptyCart(): Promise<void>;

  listPayments(): Promise<ListPayment>; 
  createPayment(payment: Payment): Promise<Payment>;
  setBillingAddress(address: Address): Promise<Order>;
  setShippingAddress(address: Address): Promise<Order>;
  setBillingAddressByID(addressID: string): Promise<Order>;
  setShippingAddressByID(addressID: string): Promise<Order>;
  onOrderChange(callback: (order: Order) => void): void;
  onLineItemsChange(callback: (lineItems: ListLineItem) => void): void;
}

export interface IProductFilters {
  toPage(pageNumber: number): void;
  sortBy(field: string): void;
  clearSort(): void;
  searchBy(searchTerm: string): void;
  clearSearch(): void;
  filterByFacet(field: string, value: string, isFacet?: boolean): void;
  clearFacetFilter(field: string): void;
  filterByCategory(categoryID: string): void;
  clearCategoryFilter(): void;
  filterByFavorites(showOnlyFavorites: boolean): void;
  clearAllFilters(): void;
  onFiltersChange(callback: (filters: ProductFilters) => void): void;
}

export interface IAuthentication {
  profiledLogin(username: string, password: string, rememberMe: boolean): Promise<AccessToken>;
  logout(): Promise<void>;
  changePassword(newPassword: string): Promise<void>;
  anonymousLogin(): Promise<AccessToken>;
  forgotPasssword(email: string): Promise<any>;
  register(me: MeUser): Promise<any>;
  resetPassword(code: string, config: PasswordReset): Promise<any>;
}

export interface ProductFilters {
  page?: number;
  sortBy?: string;
  search?: string;
  categoryID?: string;
  showOnlyFavorites?: boolean;
  activeFacets?: any;
}

export interface AuthNetCreditCard {
  CardholderName: string;
  CardNumber: string;
  ExpirationDate: string;
  SecurityCode: string;
  ID?: string;
}

export interface CreateCardResponse {
  ResponseBody: BuyerCreditCard;
  ResponseHttpStatusCode: number;
}

export interface OrderReorderResponse {
  ValidLi: Array<LineItem>;
  InvalidLi: Array<LineItem>;
}

export interface OrderDetails {
  order: Order;
  lineItems: ListLineItem;
  promotions: ListPromotion;
  payments: ListPayment;
  approvals: OrderApproval[];
}

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

  baseUrl: string;
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

