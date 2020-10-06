import { OrdercloudEnv, Theme } from 'marketplace';

export interface Environment {
  hostedApp: boolean;
  appname: string;
  clientID: string;

  // used as a prefix for order incrementor
  marketplaceID: string;
  baseUrl: string;
  middlewareUrl: string;
  sellerID: string;
  ssoLink: string;
  translateBlobUrl: string;
  ordercloudEnv: OrdercloudEnv;
  theme?: Theme;
}
