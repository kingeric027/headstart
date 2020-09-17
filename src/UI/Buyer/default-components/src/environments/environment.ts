// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.

import { OrdercloudEnv } from 'marketplace';
import { Environment } from './env.interface';
import afTheme from '../styles/themes/anytime-fitness/theme-config';
import wtcTheme from '../styles/themes/waxing-the-city/theme-config';
import bcfTheme from '../styles/themes/basecamp/theme-config';
import bmTheme from '../styles/themes/bar-method/theme-config';

// App Constants
const ANYTIME_FITNESS = 'ANYTIME_FITNESS';
type ANYTIME_FITNESS = typeof ANYTIME_FITNESS;
const WAXING_THE_CITY = 'WAXING_THE_CITY';
type WAXING_THE_CITY = typeof WAXING_THE_CITY;
const BASECAMP_FITNESS = 'BASECAMP_FITNESS';
type BASECAMP_FITNESS = typeof BASECAMP_FITNESS;
const BAR_METHOD = 'BAR_METHOD';
type BAR_METHOD = typeof BAR_METHOD;
type AppName = ANYTIME_FITNESS | WAXING_THE_CITY | BASECAMP_FITNESS | BAR_METHOD;

const LOCAL = 'LOCAL';
type LOCAL = typeof LOCAL;
const TEST = 'TEST';
type TEST = typeof TEST;
type MiddlewareLocationSelection = LOCAL | TEST;

// ===== MAKE CHANGES TO CONFIGURATION BETWEEN THESE LINES ONLY =======
// ====================================================================
const appName: AppName = BASECAMP_FITNESS;
const middlewareLocationSelection: MiddlewareLocationSelection = TEST;
const localMiddlewareURL = 'https://localhost:44334';
// ====================================================================
// ======= UNLESS YOU ARE DOING SOMETHING WEIRD =======================

// Enviroment Settings
const middlewareUrls = {
  TEST: 'https://marketplace-middleware-test.azurewebsites.net',
  LOCAL: localMiddlewareURL,
};

const devEnvironments = {
  ANYTIME_FITNESS: {
    hostedApp: false,
    appname: 'Anytime Fitness',
    clientID: 'A5231DF1-2B00-4002-AB40-738A9E2CEC4B',
    marketplaceID: 'SEB',
    baseUrl: 'https://localhost:4200',
    middlewareUrl: middlewareUrls[middlewareLocationSelection],
    translateBlobUrl: 'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
    ssoLink: null,
    ordercloudEnv: OrdercloudEnv.Staging,
    theme: afTheme,
  },
  WAXING_THE_CITY: {
    hostedApp: false,
    appname: 'Waxing The City',
    clientID: 'A5231DF1-2B00-4002-AB40-738A9E2CEC4B',
    marketplaceID: 'SEB',
    baseUrl: 'https://localhost:4200',
    middlewareUrl: middlewareUrls[middlewareLocationSelection],
    translateBlobUrl: 'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
    ssoLink: null,
    ordercloudEnv: OrdercloudEnv.Staging,
    theme: wtcTheme,
  },
  BASECAMP_FITNESS: {
    hostedApp: false,
    appname: 'Basecamp Fitness',
    clientID: 'A5231DF1-2B00-4002-AB40-738A9E2CEC4B',
    marketplaceID: 'SEB',
    baseUrl: 'https://localhost:4200',
    middlewareUrl: middlewareUrls[middlewareLocationSelection],
    translateBlobUrl: 'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
    ssoLink: null,
    ordercloudEnv: OrdercloudEnv.Staging,
    theme: bcfTheme,
  },
  BAR_METHOD: {
    hostedApp: false,
    appname: 'Bar Method',
    clientID: 'A5231DF1-2B00-4002-AB40-738A9E2CEC4B',
    marketplaceID: 'SEB',
    baseUrl: 'https://localhost:4200',
    middlewareUrl: middlewareUrls[middlewareLocationSelection],
    translateBlobUrl: 'https://marktplacetest.blob.core.windows.net/ngx-translate/i18n/',
    ssoLink: null,
    ordercloudEnv: OrdercloudEnv.Staging,
    theme: bmTheme,
  },
};
export const environment: Environment = devEnvironments[appName];
