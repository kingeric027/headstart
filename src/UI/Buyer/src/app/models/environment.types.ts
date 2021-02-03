import { ApiRole } from "ordercloud-javascript-sdk"

export enum Brand {
  'ANYTIME_FITNESS' = 'ANYTIME_FITNESS',
  'BASECAMP_FITNESS' = 'BASECAMP_FITNESS',
  'BRAND_WEAR_DESIGNS' = 'BRAND_WEAR_DESIGNS',
  'FAST_SIGNS' = 'FAST_SIGNS',
  'GO2PARTNERS' = 'GO2PARTNERS',
  'HEADSTART_DEMO' = 'HEADSTART_DEMO',
  'THEBAR_METHOD' = 'THEBAR_METHOD',
  'WAXING_THE_CITY' = 'WAXING_THE_CITY',
}

export enum Environment {
  'TEST' = 'TEST',
  'STAGING' = 'STAGING',
  'PRODUCTION' = 'PRODUCTION',
}

export enum OrdercloudEnv {
  Production = 'Production', // production and staging sites
  Staging = 'Staging', // test site && local dev
  Sandbox = 'Sandbox', // not using currently
}

export interface Theme {
  logoSrc: string
}

export interface EnvironmentConfig {
  hostedApp: boolean
  appname: string
  clientID: string
  // used as a prefix for order incrementor
  headstartID: string
  baseUrl: string
  middlewareUrl: string
  cmsUrl: string
  creditCardIframeUrl: string
  sellerID: string
  ssoLink: string
  translateBlobUrl: string
  ordercloudEnv: OrdercloudEnv
  theme?: Theme
  instrumentationKey: string
}

export class AppConfig {
  /**
   * A short name for your app. It will be used as a
   * cookie prefix as well as general display throughout the app.
   */
  appname: string
  /**
   * The identifier for the seller, buyer network or buyer application that
   * will be used for authentication. You can view client ids for apps
   * you own or are a contributor to on the [dashboard](https://developer.ordercloud.io/dashboard)
   */
  clientID: string
  /**
   * The identifier for the headstart org.
   */
  headstartID: string
  /**
   * If set to true users can browse and submit orders without profiling themselves. This requires
   * additional set up in the dashboard. Click here to
   * [learn more](https://developer.ordercloud.io/documentation/platform-guides/authentication/anonymous-shopping)
   */
  anonymousShoppingEnabled: boolean
  baseUrl: string
  /**
   * base path to middleware
   */
  translateBlobUrl: string
  ordercloudEnv: OrdercloudEnv
  cmsUrl: string
  middlewareUrl: string
  creditCardIframeUrl: string
  /**
   *  The ID of the seller organization.
   */
  sellerID: string
  /**
   *  TODO - Link to identity provider's authorization server. this field should probably be SEB-specific.
   */
  ssoLink: string
  /**
   * An array of security roles that will be requested upon login.
   * These roles allow access to specific endpoints in the OrderCloud.io API.
   * To learn more about these roles and the security profiles that comprise them
   * read [here](https://developer.ordercloud.io/documentation/platform-guides/authentication/security-profiles)
   */
  scope: ApiRole[]
  theme: Theme
  /**
   * Microsoft Azure Application Insights instrumentation key
   */
  instrumentationKey: string
}