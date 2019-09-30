import { ocAppConfig } from 'src/app/config/app.config';
import { Configuration } from '@ordercloud/angular-sdk';

export const OcSDKConfig = new Configuration({
  basePath: 'https://api.ordercloud.io/v1',
  authPath: 'https://auth.ordercloud.io/oauth/token',
  cookiePrefix: ocAppConfig.appname.replace(/ /g, '_').toLowerCase(),
});
