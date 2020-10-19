import { Environment } from '../environment.interface';

export const environment: Environment = {
    hostedApp: true,
    sellerID: 'GO2PARTNERS_TEST',
    clientID: '1C414E48-B027-4C6D-85CF-873723EB7A70',
    middlewareUrl: 'https://go2partners-middleware-test.azurewebsites.net',
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