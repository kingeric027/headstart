// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// TODO: Change oc integration middleware url to staging URL when one exists
export const environment = {
  hostedApp: true,
  clientID: 'C1D92C43-C7AF-467B-B3DF-D4751D70CAE6',
  middlewareUrl: 'https://marketplace-middleware.azurewebsites.net',
  appname: 'Self Esteem Brands',
  translateBlobUrl: 'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
  orderCloudApiUrl: 'https://api.ordercloud.io',
  orderCloudApiVersion: 'v1',
};