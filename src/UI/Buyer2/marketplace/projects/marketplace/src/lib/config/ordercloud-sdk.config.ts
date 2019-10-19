import { Configuration, OrderCloudModule } from '@ordercloud/angular-sdk';
import { ocAppConfig } from './app.config';
import { InjectionToken } from '@angular/core';


export function OcSDKConfig() {
  return OCConfig;
}

export const OCConfig = new Configuration({
  basePath: 'https://api.ordercloud.io/v1',
  authPath: 'https://auth.ordercloud.io/oauth/token',
  cookiePrefix: ocAppConfig.appname.replace(/ /g, '_').toLowerCase(),
});
