import { Configuration } from '@ordercloud/angular-sdk';
import { InjectionToken } from '@angular/core';

export const ROOT_REDUCER = new InjectionToken<any>('Root Reducer');

export function OcSDKConfig() {
  return OCConfig;
}

export const OCConfig = new Configuration({
  basePath: 'https://api.ordercloud.io/v1',
  authPath: 'https://auth.ordercloud.io/oauth/token',
  // cookiePrefix: ocAppConfig.appname.replace(/ /g, '_').toLowerCase(),
  cookiePrefix: 'Ordercloud'.replace(/ /g, '_').toLowerCase(),
});
