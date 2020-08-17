// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.

export const environment = {
  hostedApp: false,
  clientID: '06C93629-FE9A-4EC5-9652-C0F059B5CC7C',
  middlewareUrl: 'https://localhost:44384',
  appname: 'Marketplace Admin Local',
  translateBlobUrl: 'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
  orderCloudApiUrl: 'https://stagingapi.ordercloud.io',
  orderCloudApiVersion: 'v1',
  buyerUrl: 'https://marketplace-buyer-ui-test.azurewebsites.net/',
  buyerClientID: 'A5231DF1-2B00-4002-AB40-738A9E2CEC4B'
};
