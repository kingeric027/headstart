// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --configuration=qa` replaces `environment.ts` with `environment.qa.ts`.
// The list of file replacements can be found in `angular.json`.

import { Environment } from './env.interface';
import { OrdercloudEnv } from 'marketplace';

export const environment: Environment = {
  hostedApp: true,
  appname: 'Print Marketplace Test',
  clientID: '4B4C2ECA-A21B-47DD-A974-5BA7FCDF0EDE',
  marketplaceID: 'SEB',
  baseUrl: 'https://localhost:4200',
  middlewareUrl: 'https://marketplace-middleware-test.azurewebsites.net',
  ssoLink:
    'https://stage-authorize.anytimefitness.com/authorize?response_type=code&client_id=86d70db9-22e6-47ba-a1ab-bbe00c9b6451&redirect_uri=https://selfesteembrands-api-qa.azurewebsites.net/authorize',
  ordercloudEnv: OrdercloudEnv.Staging,
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
