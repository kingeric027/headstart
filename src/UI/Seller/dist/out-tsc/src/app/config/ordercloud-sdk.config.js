import { ocAppConfig } from '@app-seller/config/app.config';
import { Configuration } from '@ordercloud/angular-sdk';
export function OcSDKConfig() {
    var apiurl = 'https://api.ordercloud.io';
    var apiVersion = 'v1';
    var authUrl = 'https://auth.ordercloud.io/oauth/token';
    return new Configuration({
        basePath: apiurl + "/" + apiVersion,
        authPath: authUrl,
        cookiePrefix: ocAppConfig.appname.replace(/ /g, '_').toLowerCase(),
    });
}
//# sourceMappingURL=ordercloud-sdk.config.js.map