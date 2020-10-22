// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.'

import { Environment } from './environment.interface';
export const environment: Environment = {
  hostedApp: true,
  clientID: 'C1D92C43-C7AF-467B-B3DF-D4751D70CAE6',
  sellerID: 'pPOiukEUHkSGrBmAIjdReQ',
  middlewareUrl: 'https://marketplace-middleware-staging.azurewebsites.net',
  appname: 'Marketplace Admin Staging',
  translateBlobUrl: 'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
  blobStorageUrl: 'https://marktplacestaging.blob.core.windows.net',
  orderCloudApiUrl: 'https://stagingapi.ordercloud.io',
  orderCloudApiVersion: 'v1',
  buyerConfigs: {
    'Anytime Fitness': {
      clientID: 'F18AE28D-FFA4-4A5A-9C69-A1FBC71DCD3D',
      buyerUrl: 'https://anytimefitness-staging.sebvendorportal.com'
    },
    'Basecamp Fitness': {
      clientID: '0045BB67-84CC-42BD-9FA7-009875628F7C',
      buyerUrl: 'https://basecampfitness-staging.sebvendorportal.com'
    },
    'The Bar Method': {
      clientID: 'CF547B04-1826-427D-9940-824805F0ECA0',
      buyerUrl: 'https://thebarmethod-staging.sebvendorportal.com'
    },
    'Waxing The City': {
      clientID: '0BF4E739-7C2A-45A9-9A08-2AD44EB75F1D',
      buyerUrl: 'https://waxingthecity-staging.sebvendorportal.com'
    }
  },
  superProductFieldsToMonitor: ['PriceSchedule']
};
