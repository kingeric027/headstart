// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --configuration=demo` replaces `environment.ts` with `environment.demo.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  hostedApp: true,
  appname: 'Marketplace Staging',
  clientID: 'A5231DF1-2B00-4002-AB40-738A9E2CEC4B',
  marketplaceID: 'SEB',
  baseUrl: 'http://marketplace-buyer-ui-staging.azurewebsites.net/',
  middlewareUrl: 'https://marketplace-middleware-staging.azurewebsites.net',
  cmsUrl: 'https://s3.dualstack.us-east-1.amazonaws.com/staticcintas.eretailing.com/images/product',
  ssoLink:
    'https://stage-authorize.anytimefitness.com/authorize?response_type=code&client_id=86d70db9-22e6-47ba-a1ab-bbe00c9b6451&redirect_uri=https://selfesteembrands-api-qa.azurewebsites.net/authorize',
  orderCloudApiUrl: 'https://api.ordercloud.io',
  orderCloudAuthUrl: 'https://auth.ordercloud.io/oauth/token',
  orderCloudApiVersion: 'v1',
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
