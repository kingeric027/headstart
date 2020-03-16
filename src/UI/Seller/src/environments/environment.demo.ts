// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.

export const environment = {
  hostedApp: true,
  clientID: '06C93629-FE9A-4EC5-9652-C0F059B5CC7C',
  middlewareUrl: 'https://marketplace-api-qa.azurewebsites.net',
  appname: 'Marketplace Admin Demo',
  marketplaceID: 'seb',
  cmsUrl: 'https://marketplaceqa.blob.core.windows.net',
  orderCloudApiUrl: 'https://api.ordercloud.io',
  orderCloudAuthUrl: 'https://auth.ordercloud.io/oauth/token',
  orderCloudApiVersion: 'v1',
};
