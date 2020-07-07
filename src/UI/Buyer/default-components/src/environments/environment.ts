// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.

import { OrdercloudEnv } from 'marketplace';

// The list of file replacements can be found in `angular.json`.
export interface Environment {
  hostedApp: boolean;
  appname: string;
  clientID: string;
  marketplaceID: string;
  baseUrl: string;
  middlewareUrl: string;
  ssoLink: string;
  ordercloudEnv: OrdercloudEnv;
}

export const environment: Environment = {
  hostedApp: false,
  appname: 'Marketplace Local',
  clientID: 'A5231DF1-2B00-4002-AB40-738A9E2CEC4B',
  marketplaceID: 'SEB',
  baseUrl: 'https://localhost:4200',
  middlewareUrl: 'https://localhost:44314',
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
