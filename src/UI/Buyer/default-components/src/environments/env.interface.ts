import { OrdercloudEnv, Theme } from 'marketplace';

export interface Environment {
  hostedApp: boolean;
  appname: string;
  clientID: string;
  marketplaceID: string;
  baseUrl: string;
  middlewareUrl: string;
  creditCardIframeUrl: string;
  sellerID: string;
  ssoLink: string;
  translateBlobUrl: string;
  ordercloudEnv: OrdercloudEnv;
  theme?: Theme;
  instrumentationKey: string;
}
