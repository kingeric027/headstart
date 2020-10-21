import { Environment } from './environment.interface';

export const environment = {
  hostedApp: false,
  sellerID: 'rQYR6T6ZTEqVrgv8x_ei0g',
  clientID: '06C93629-FE9A-4EC5-9652-C0F059B5CC7C',
  middlewareUrl: 'https://localhost:44385',
  appname: 'Marketplace Admin Local',
  translateBlobUrl: 'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
  blobStorageUrl: 'https://marktplacetest.blob.core.windows.net',
  orderCloudApiUrl: 'https://stagingapi.ordercloud.io',
  orderCloudApiVersion: 'v1',
  buyerConfigs: {
    'Anytime Fitness': {
      clientID: 'A5231DF1-2B00-4002-AB40-738A9E2CEC4B',
      buyerUrl: 'https://marketplace-buyer-ui-test.azurewebsites.net/'
    },
    superProductFieldsToMonitor: ['PriceSchedule', 'Product.xp.IsResale']
  },
  BRANDWEAR_DESIGNS: {
    hostedApp: false,
    sellerID: 'rQYR6T6ZTEqVrgv8x_ei0g',
    clientID: '06C93629-FE9A-4EC5-9652-C0F059B5CC7C',
    middlewareUrl: middlewareLocationSelection === 'LOCAL' as any ? localMiddlewareURL : 'https://brandweardesigns-middleware-test.azurewebsites.net',
    appname: 'Marketplace Admin',
    translateBlobUrl: 'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
    blobStorageUrl: 'https://marktplacetest.blob.core.windows.net',
    orderCloudApiUrl: 'https://stagingapi.ordercloud.io',
    orderCloudApiVersion: 'v1',
    buyerConfigs: {
      'Default Buyer': {
        clientID: 'A5231DF1-2B00-4002-AB40-738A9E2CEC4B',
        buyerUrl: 'https://marketplace-buyer-ui-test.azurewebsites.net/',
      }
    },
    superProductFieldsToMonitor: []
  },
  HEADSTART_DEMO: {
    hostedApp: false,
    sellerID: 'Headstart_Demo_Test',
    clientID: 'FF151BA7-0207-4134-8A7D-52235BEE4E7A',
    middlewareUrl: middlewareLocationSelection === 'LOCAL' as any ? localMiddlewareURL : 'https://headstartdemo-middleware-test.azurewebsites.net',
    appname: 'Headstart Demo Admin',
    translateBlobUrl: 'https://stfour51demotest.blob.core.windows.net/ngx-translate2/i18n/',
    blobStorageUrl: 'https://stfour51demotest.blob.core.windows.net',
    orderCloudApiUrl: 'https://sandboxapi.ordercloud.io',
    orderCloudApiVersion: 'v1',
    buyerConfigs: {
      'Default Buyer': {
        clientID: 'A482C18B-527B-4BA1-A2E9-0E7C65C2E39F',
        buyerUrl: 'https://headstartdemo-buyer-ui-test.azurewebsites.net/',
      }
    },
    superProductFieldsToMonitor: []
  },
  FAST_SIGNS: {
    hostedApp: false,
    sellerID: 'FASTSIGNS_TEST',
    clientID: 'E8B2E3BD-2FAE-4C26-9BE3-054953393C2C',
    middlewareUrl: middlewareLocationSelection === 'LOCAL' as any ? localMiddlewareURL : 'https://fastsigns-middleware-test.azurewebsites.net',
    appname: 'FASTSIGNS Admin',
    translateBlobUrl: 'https://stfastsignstest.blob.core.windows.net/ngx-translate/i18n/',
    blobStorageUrl: 'https://stfastsignstest.blob.core.windows.net',
    orderCloudApiUrl: 'https://sandboxapi.ordercloud.io',
    orderCloudApiVersion: 'v1',
    buyerConfigs: {
      'Default Buyer': {
        clientID: '3B7CD2F7-36D8-4DC4-9616-0CB1C86C9FB3',
        buyerUrl: 'https://fastsigns-buyer-ui-test.azurewebsites.net/'
      }
    },
    superProductFieldsToMonitor: []
  },
  GO2PARTNERS: {
    hostedApp: false,
    sellerID: 'GO2PARTNERS_TEST',
    clientID: '1C414E48-B027-4C6D-85CF-873723EB7A70',
    middlewareUrl: middlewareLocationSelection === 'LOCAL' as any ? localMiddlewareURL : 'https://go2partners-middleware-test.azurewebsites.net',
    appname: 'GO2 Partners Admin',
    translateBlobUrl: 'https://stgo2partnerstest.blob.core.windows.net/ngx-translate/i18n/',
    blobStorageUrl: 'https://stgo2partnerstest.blob.core.windows.net',
    orderCloudApiUrl: 'https://sandboxapi.ordercloud.io',
    orderCloudApiVersion: 'v1',
    buyerConfigs: {
      'Default Buyer': {
        clientID: 'B1FEB16F-9E3E-4534-88FE-F3AE29941986',
        buyerUrl: 'https://go2partners-buyer-ui-test.azurewebsites.net/'
      }
    },
    superProductFieldsToMonitor: []
  }
};
export const environment: Environment = devEnvironments[appName];