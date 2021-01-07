import { EnvironmentConfig } from '@app-seller/models/environment.types'

export const environment: EnvironmentConfig = {
  hostedApp: true,
  sellerID: 'Headstart_Demo_Test',
  sellerName: 'SEB Seller',
  clientID: 'FF151BA7-0207-4134-8A7D-52235BEE4E7A',
  middlewareUrl: 'https://headstartdemo-middleware-test.azurewebsites.net',
  cmsUrl: 'https://ordercloud-cms-test.azurewebsites.net',
  appname: 'Headstart Demo Admin',
  translateBlobUrl:
    'https://stfour51demotest.blob.core.windows.net/ngx-translate2/i18n/',
  blobStorageUrl: 'https://stfour51demotest.blob.core.windows.net',
  orderCloudApiUrl: 'https://sandboxapi.ordercloud.io',
  orderCloudApiVersion: 'v1',
  buyerConfigs: {
    'Default Buyer': {
      clientID: 'A482C18B-527B-4BA1-A2E9-0E7C65C2E39F',
      buyerUrl: 'https://headstartdemo-buyer-ui-test.azurewebsites.net/',
    },
  },
  superProductFieldsToMonitor: [],
}
