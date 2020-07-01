// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.

export const environment = {
  hostedApp: false,
  clientID: '06C93629-FE9A-4EC5-9652-C0F059B5CC7C',
  middlewareUrl: 'https://marketplace-middleware-test.azurewebsites.net',
  ocMiddlewareUrl: 'https://ordercloud-middleware-test.azurewebsites.net',
  appname: 'Marketplace Admin Local',
  marketplaceID: 'SEB',
  cmsUrl: 'https://marktplacetest.blob.core.windows.net',
  orderCloudApiUrl: 'https://stagingapi.ordercloud.io',
  orderCloudAuthUrl: 'https://stagingauth.ordercloud.io/oauth/token',
  orderCloudApiVersion: 'v1',
};
