import { EnvironmentConfig } from './environment.interfaces'

export const environment: EnvironmentConfig = {
  hostedApp: true,
  sellerID: 'FASTSIGNS_TEST',
  sellerName: 'FastSigns Seller',
  clientID: 'E8B2E3BD-2FAE-4C26-9BE3-054953393C2C',
  middlewareUrl: 'https://fastsigns-middleware-test.azurewebsites.net',
  cmsUrl: 'https://ordercloud-cms-test.azurewebsites.net',
  appname: 'FASTSIGNS Admin',
  translateBlobUrl:
    'https://stfastsignstest.blob.core.windows.net/ngx-translate/i18n/',
  blobStorageUrl: 'https://stfastsignstest.blob.core.windows.net',
  orderCloudApiUrl: 'https://sandboxapi.ordercloud.io',
  orderCloudApiVersion: 'v1',
  buyerConfigs: {
    'Default Buyer': {
      clientID: '3B7CD2F7-36D8-4DC4-9616-0CB1C86C9FB3',
      buyerUrl: 'https://fastsigns-buyer-ui-test.azurewebsites.net/',
    },
  },
  superProductFieldsToMonitor: [],
}
