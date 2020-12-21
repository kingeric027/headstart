import { OrdercloudEnv, Theme } from 'src/app/shopper-context'

export interface EnvironmentConfig {
  hostedApp: boolean
  appname: string
  clientID: string
  // used as a prefix for order incrementor
  marketplaceID: string
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
