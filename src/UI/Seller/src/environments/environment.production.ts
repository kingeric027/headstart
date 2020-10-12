// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// TODO: Change oc integration middleware url to staging URL when one exists
export const environment = {
  hostedApp: true,
  sellerID: 'pPOiukEUHkSGrBmAIjdReQ',
  clientID: 'C1D92C43-C7AF-467B-B3DF-D4751D70CAE6',
  middlewareUrl: 'https://marketplace-middleware.azurewebsites.net',
  appname: 'Self Esteem Brands',
  translateBlobUrl: 'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
  blobStorageUrl: 'https://marktplacetest.blob.core.windows.net',
  orderCloudApiUrl: 'https://api.ordercloud.io',
  orderCloudApiVersion: 'v1',
  buyerConfigs: {
    'Anytime Fitness': {
      clientID: 'F18AE28D-FFA4-4A5A-9C69-A1FBC71DCD3D',
      buyerUrl: 'http://marketplace-buyer-ui.azurewebsites.net/'
    },
    'Basecamp Fitness': {
      clientID: '0045BB67-84CC-42BD-9FA7-009875628F7C',
      buyerUrl: 'http://marketplace-buyer-ui.azurewebsites.net/'
    },
    'The Bar Method': {
      clientID: 'CF547B04-1826-427D-9940-824805F0ECA0',
      buyerUrl: 'http://marketplace-buyer-ui.azurewebsites.net/'
    },
    'Waxing The City': {
      clientID: '0BF4E739-7C2A-45A9-9A08-2AD44EB75F1D',
      buyerUrl: 'http://marketplace-buyer-ui.azurewebsites.net/'
    }
  },
  productFieldsToMonitor: ["PriceSchedule"]
};
