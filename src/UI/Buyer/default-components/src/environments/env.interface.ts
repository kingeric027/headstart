import { OrdercloudEnv } from 'marketplace';

export interface Environment {
  hostedApp: boolean;
  appname: string;
  clientID: string;
  marketplaceID: string;
  baseUrl: string;
  middlewareUrl: string;
  ssoLink: string;
  ordercloudEnv: OrdercloudEnv;
}
