// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// TODO: Change oc integration middleware url to staging URL when one exists
export const environment = {
  hostedApp: true,
  clientID: 'C1D92C43-C7AF-467B-B3DF-D4751D70CAE6',
  sellerID: 'pPOiukEUHkSGrBmAIjdReQ',
  middlewareUrl: 'https://marketplace-middleware-staging.azurewebsites.net',
  appname: 'Marketplace Admin Staging',
  translateBlobUrl: 'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
  blobStorageUrl: 'https://marktplacetest.blob.core.windows.net',
  orderCloudApiUrl: 'https://stagingapi.ordercloud.io',
  orderCloudApiVersion: 'v1',
  buyerUrl: 'https://marketplace-buyer-ui-staging.azurewebsites.net/',
  buyerClientID: 'A5231DF1-2B00-4002-AB40-738A9E2CEC4B',
};
