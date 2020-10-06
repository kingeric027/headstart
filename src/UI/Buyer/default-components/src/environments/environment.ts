// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.

import { OrdercloudEnv } from 'marketplace';
import { Environment } from './env.interface';
import afTheme from '../styles/themes/anytime-fitness/theme-config';
import wtcTheme from '../styles/themes/waxing-the-city/theme-config';
import bcfTheme from '../styles/themes/basecamp/theme-config';
import bmTheme from '../styles/themes/bar-method/theme-config';
import headstartdemoTheme from '../styles/themes/headstartdemo/theme-config';
import fastsignsTheme from '../styles/themes/fastsigns/theme-config';
import go2partnersTheme from '../styles/themes/go2partners/theme-config';
import brandweardesignsTheme from '../styles/themes/brandweardesigns/theme-config';

// App Constants
const ANYTIME_FITNESS = 'ANYTIME_FITNESS';
type ANYTIME_FITNESS = typeof ANYTIME_FITNESS;
const WAXING_THE_CITY = 'WAXING_THE_CITY';
type WAXING_THE_CITY = typeof WAXING_THE_CITY;
const BASECAMP_FITNESS = 'BASECAMP_FITNESS';
type BASECAMP_FITNESS = typeof BASECAMP_FITNESS;
const BAR_METHOD = 'BAR_METHOD';
type BAR_METHOD = typeof BAR_METHOD;
const HEADSTART_DEMO = 'HEADSTART_DEMO';
type HEADSTART_DEMO = typeof HEADSTART_DEMO;
const FASTSIGNS = 'FASTSIGNS';
type FASTSIGNS = typeof FASTSIGNS;
const BRANDWEAR_DESIGNS = 'BRANDWEAR_DESIGNS'
type BRANDWEAR_DESIGNS = typeof BRANDWEAR_DESIGNS;
const GO2PARTNERS = 'GO2PARTNERS';
type GO2PARTNERS = typeof GO2PARTNERS;
type AppName = ANYTIME_FITNESS | WAXING_THE_CITY | BASECAMP_FITNESS | BAR_METHOD | HEADSTART_DEMO |
  FASTSIGNS | BRANDWEAR_DESIGNS | GO2PARTNERS;

const LOCAL = 'LOCAL';
type LOCAL = typeof LOCAL;
const TEST = 'TEST';
type TEST = typeof TEST;
type MiddlewareLocationSelection = LOCAL | TEST;

// ===== MAKE CHANGES TO CONFIGURATION BETWEEN THESE LINES ONLY =======
// ====================================================================
const appName: AppName = GO2PARTNERS;
const middlewareLocationSelection: MiddlewareLocationSelection = LOCAL;
const localMiddlewareURL = 'https://localhost:5001';
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
    sellerID: 'rQYR6T6ZTEqVrgv8x_ei0g',
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
    sellerID: 'rQYR6T6ZTEqVrgv8x_ei0g',
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
    sellerID: 'rQYR6T6ZTEqVrgv8x_ei0g',
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
    sellerID: 'rQYR6T6ZTEqVrgv8x_ei0g',
    ssoLink: null,
    ordercloudEnv: OrdercloudEnv.Staging,
    theme: bmTheme,
  },
  HEADSTART_DEMO: {
    hostedApp: false,
    appname: 'Headstart Demo',
    clientID: 'A482C18B-527B-4BA1-A2E9-0E7C65C2E39F',
    marketplaceID: 'DEMO',
    baseUrl: 'https://localhost:4200',
    middlewareUrl: middlewareUrls[middlewareLocationSelection],
    translateBlobUrl: 'https://stfour51demotest.blob.core.windows.net/ngx-translate2/i18n/',
    sellerID: 'Headstart_Demo_Test',
    ssoLink: null,
    ordercloudEnv: OrdercloudEnv.Sandbox,
    theme: headstartdemoTheme,
  },
  FASTSIGNS: {
    hostedApp: false,
    appname: 'FASTSIGNS',
    clientID: '3B7CD2F7-36D8-4DC4-9616-0CB1C86C9FB3',
    marketplaceID: 'FS',
    baseUrl: 'https://localhost:4200',
    middlewareUrl: middlewareUrls[middlewareLocationSelection],
    translateBlobUrl: 'https://stfastsignstest.blob.core.windows.net/ngx-translate/i18n/',
    sellerID: 'FASTSIGNS_TEST',
    ssoLink: null,
    ordercloudEnv: OrdercloudEnv.Sandbox,
    theme: fastsignsTheme,
  },
  GO2PARTNERS: {
    hostedApp: false,
    appname: 'GO2 Partners',
    clientID: 'B1FEB16F-9E3E-4534-88FE-F3AE29941986',
    marketplaceID: 'GO2',
    baseUrl: 'https://localhost:4200',
    middlewareUrl: middlewareUrls[middlewareLocationSelection],
    translateBlobUrl: 'https://stgo2partnerstest.blob.core.windows.net/ngx-translate/i18n/',
    sellerID: 'GO2PARTNERS_TEST',
    ssoLink: null,
    ordercloudEnv: OrdercloudEnv.Sandbox,
    theme: go2partnersTheme,
  },
  BRANDWEAR_DESIGNS: {
    hostedApp: false,
    appname: 'BRANDWEAR Designs',
    clientID: '2F33BE12-D914-419C-B3D0-41AEFB72BE93',
    marketplaceID: 'BW',
    baseUrl: 'https://localhost:4200',
    middlewareUrl: middlewareUrls[middlewareLocationSelection],
    translateBlobUrl: 'https://stbrandweartest.blob.core.windows.net/ngx-translate/i18n/',
    sellerID: 'BRANDWEARDESIGNS_TEST',
    ssoLink: null,
    ordercloudEnv: OrdercloudEnv.Sandbox,
    theme: brandweardesignsTheme,
  }
};
export const environment: Environment = devEnvironments[appName];
