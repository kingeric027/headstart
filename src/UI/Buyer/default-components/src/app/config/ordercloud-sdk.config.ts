import { Configuration } from '@ordercloud/angular-sdk';
import { environment } from 'src/environments/environment';

export function OcSDKConfig(): Configuration {
  return new Configuration({
    basePath: `${environment.orderCloudApiUrl}/${environment.orderCloudApiVersion}`,
    authPath: `${environment.orderCloudAuthUrl}`,
    cookiePrefix: environment.appname.replace(/ /g, '_').toLowerCase(),
  });
}
