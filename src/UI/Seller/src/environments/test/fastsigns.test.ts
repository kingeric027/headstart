import { Environment } from '../environment.interface';

export const environment: Environment = {
    hostedApp: true,
    sellerID: 'FASTSIGNS_TEST',
    clientID: 'E8B2E3BD-2FAE-4C26-9BE3-054953393C2C',
    middlewareUrl: 'https://fastsigns-middleware-test.azurewebsites.net',
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
}