import { Environment } from './environment.interface';

// App Constants
const BRANDWEAR_DESIGNS = 'BRANDWEAR_DESIGNS';
type BRANDWEAR_DESIGNS = typeof BRANDWEAR_DESIGNS;
const FAST_SIGNS = 'FAST_SIGNS';
type FAST_SIGNS = typeof FAST_SIGNS;
const HEADSTART_DEMO = 'HEADSTART_DEMO';
type HEADSTART_DEMO = typeof HEADSTART_DEMO;
const SELF_ESTEEM_BRANDS = 'SELF_ESTEEM_BRANDS';
type SELF_ESTEEM_BRANDS = typeof SELF_ESTEEM_BRANDS;
const GO2PARTNERS = 'GO2PARTNERS';
type GO2PARTNERS = typeof GO2PARTNERS;

type AppName = BRANDWEAR_DESIGNS | FAST_SIGNS | HEADSTART_DEMO | SELF_ESTEEM_BRANDS | GO2PARTNERS;

const LOCAL = 'LOCAL';
type LOCAL = typeof LOCAL;
const TEST = 'TEST';
type TEST = typeof TEST;
type MiddlewareLocationSelection = LOCAL | TEST;

// ===== MAKE CHANGES TO CONFIGURATION BETWEEN THESE LINES ONLY =======
// ====================================================================
const appName: AppName = FAST_SIGNS;
const middlewareLocationSelection: MiddlewareLocationSelection = LOCAL;
const localMiddlewareURL = 'https://localhost:5001';
// ====================================================================
// ======= UNLESS YOU ARE DOING SOMETHING WEIRD =======================

const devEnvironments = {
  BRANDWEAR_DESIGNS: {
    hostedApp: false,
    sellerID: 'rQYR6T6ZTEqVrgv8x_ei0g',
    clientID: '06C93629-FE9A-4EC5-9652-C0F059B5CC7C',
    middlewareUrl: middlewareLocationSelection === 'LOCAL' ? localMiddlewareURL : 'https://marketplace-middleware-test.azurewebsites.net',
    appname: 'Marketplace Admin',
    translateBlobUrl: 'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
    blobStorageUrl: 'https://marktplacetest.blob.core.windows.net',
    orderCloudApiUrl: 'https://stagingapi.ordercloud.io',
    orderCloudApiVersion: 'v1',
    buyerUrl: 'https://marketplace-buyer-ui-test.azurewebsites.net/',
    buyerClientID: 'A5231DF1-2B00-4002-AB40-738A9E2CEC4B',
  },
  HEADSTART_DEMO: {
    hostedApp: false,
    sellerID: 'Headstart_Demo_Test',
    clientID: 'FF151BA7-0207-4134-8A7D-52235BEE4E7A',
    middlewareUrl: middlewareLocationSelection === 'LOCAL' ? localMiddlewareURL : 'https://headstartdemo-middleware-test.azurewebsites.net',
    appname: 'Headstart Demo Admin',
    translateBlobUrl: 'https://stfour51demotest.blob.core.windows.net/ngx-translate2/i18n/',
    blobStorageUrl: 'https://stfour51demotest.blob.core.windows.net',
    orderCloudApiUrl: 'https://sandboxapi.ordercloud.io',
    orderCloudApiVersion: 'v1',
    buyerUrl: 'https://headstartdemo-buyer-ui-test.azurewebsites.net/',
    buyerClientID: 'A482C18B-527B-4BA1-A2E9-0E7C65C2E39F',
  },
  FAST_SIGNS: {
    hostedApp: false,
    sellerID: 'FASTSIGNS_TEST',
    clientID: 'E8B2E3BD-2FAE-4C26-9BE3-054953393C2C',
    middlewareUrl: middlewareLocationSelection === 'LOCAL' ? localMiddlewareURL : 'https://fastsigns-middleware-test.azurewebsites.net',
    appname: 'FASTSIGNS Admin',
    translateBlobUrl: 'https://stfastsignstest.blob.core.windows.net/ngx-translate/i18n/',
    blobStorageUrl: 'https://stfastsignstest.blob.core.windows.net',
    orderCloudApiUrl: 'https://sandboxapi.ordercloud.io',
    orderCloudApiVersion: 'v1',
    buyerUrl: 'https://fastsigns-buyer-ui-test.azurewebsites.net/',
    buyerClientID: '3B7CD2F7-36D8-4DC4-9616-0CB1C86C9FB3',
  },
  GO2PARTNERS: {
    hostedApp: false,
    sellerID: 'GO2PARTNERS_TEST',
    clientID: '1C414E48-B027-4C6D-85CF-873723EB7A70',
    middlewareUrl: middlewareLocationSelection === 'LOCAL' ? localMiddlewareURL : 'https://go2partners-middleware-test.azurewebsites.net',
    appname: 'GO2 Partners Admin',
    translateBlobUrl: 'https://stgo2partnerstest.blob.core.windows.net/ngx-translate/i18n/',
    blobStorageUrl: 'https://stgo2partnerstest.blob.core.windows.net',
    orderCloudApiUrl: 'https://sandboxapi.ordercloud.io',
    orderCloudApiVersion: 'v1',
    buyerUrl: 'https://go2partners-buyer-ui-test.azurewebsites.net/',
    buyerClientID: 'B1FEB16F-9E3E-4534-88FE-F3AE29941986',
  },
  SELF_ESTEEM_BRANDS: {
    hostedApp: false,
    sellerID: 'rQYR6T6ZTEqVrgv8x_ei0g',
    clientID: '06C93629-FE9A-4EC5-9652-C0F059B5CC7C',
    middlewareUrl: middlewareLocationSelection === 'LOCAL' ? localMiddlewareURL : 'https://marketplace-middleware-test.azurewebsites.net',
    appname: 'Marketplace Admin Test',
    translateBlobUrl: 'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
    blobStorageUrl: 'https://marktplacetest.blob.core.windows.net',
    orderCloudApiUrl: 'https://stagingapi.ordercloud.io',
    orderCloudApiVersion: 'v1',
    buyerUrl: 'https://marketplace-buyer-ui-test.azurewebsites.net/',
    buyerClientID: 'A5231DF1-2B00-4002-AB40-738A9E2CEC4B',
  }
};
export const environment: Environment = devEnvironments[appName];
