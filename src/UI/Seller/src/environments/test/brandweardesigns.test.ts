import { Environment } from '../environment.interface';

export const environment: Environment = {
    hostedApp: true,
    sellerID: 'BRANDWEARDESIGNS_TEST',
    clientID: '13FB36D2-52CA-44D7-B426-2B4BB474EB47',
    middlewareUrl: 'https://brandweardesigns-middleware-test.azurewebsites.net',
    appname: 'BRANDWEAR Designs Admin',
    translateBlobUrl: 'https://stbrandweartest.blob.core.windows.net/ngx-translate/i18n/',
    blobStorageUrl: 'https://stbrandweartest.blob.core.windows.net',
    orderCloudApiUrl: 'https://sandboxapi.ordercloud.io',
    orderCloudApiVersion: 'v1',
    buyerConfigs: {
        'Default Buyer': {
            clientID: '2F33BE12-D914-419C-B3D0-41AEFB72BE93',
            buyerUrl: 'https://brandweardesigns-buyer-ui-test.azurewebsites.net/'
        }
    },
    superProductFieldsToMonitor: []
}