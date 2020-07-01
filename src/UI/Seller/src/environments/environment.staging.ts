// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
//TODO: Change oc integration middleware url to staging URL when one exists
export const environment = {
  hostedApp: true,
  clientID: 'AAADD078-E169-4610-816A-C760205D1365',
  middlewareUrl: 'https://marketplace-middleware-staging.azurewebsites.net',
  ocMiddlewareUrl: 'https://ordercloud-middleware-test.azurewebsites.net',
  appname: 'Marketplace Admin Staging',
  marketplaceID: 'SEB',
  cmsUrl: 'https://marktplacestaging.blob.core.windows.net',
  orderCloudApiUrl: 'https://api.ordercloud.io',
  orderCloudAuthUrl: 'https://auth.ordercloud.io/oauth/token',
  orderCloudApiVersion: 'v1',
};
