import { OrdercloudEnv, Theme } from 'src/app/shopper-context';

export interface Environment {
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