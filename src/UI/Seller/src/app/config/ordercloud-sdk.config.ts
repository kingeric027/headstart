import { Configuration } from '@ordercloud/angular-sdk';
import { environment } from '../../environments/environment';

export const OcSDKConfig = (): Configuration => {
  return new Configuration({
    basePath: `${environment.orderCloudApiUrl}/${environment.orderCloudApiVersion}`,
    authPath: `${environment.orderCloudAuthUrl}`,
    cookiePrefix: environment.appname.replace(/ /g, '_').toLowerCase(),
  });
};
