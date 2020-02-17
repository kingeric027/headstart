import { __decorate, __metadata, __param } from "tslib";
import { Injectable, Inject } from '@angular/core';
import { OcTokenService } from '@ordercloud/angular-sdk';
import { applicationConfiguration } from '@app-seller/config/app.config';
/**
 * automatically append token to the authorization header
 * required to interact with middleware layer
 */
var AutoAppendTokenInterceptor = /** @class */ (function () {
    function AutoAppendTokenInterceptor(ocTokenService, appConfig) {
        this.ocTokenService = ocTokenService;
        this.appConfig = appConfig;
    }
    AutoAppendTokenInterceptor.prototype.intercept = function (request, next) {
        if (request.url.includes(this.appConfig.middlewareUrl)) {
            request = request.clone({
                setHeaders: {
                    Authorization: "Bearer " + this.ocTokenService.GetAccess(),
                },
            });
        }
        return next.handle(request);
    };
    AutoAppendTokenInterceptor = __decorate([
        Injectable({
            providedIn: 'root',
        }),
        __param(1, Inject(applicationConfiguration)),
        __metadata("design:paramtypes", [OcTokenService, Object])
    ], AutoAppendTokenInterceptor);
    return AutoAppendTokenInterceptor;
}());
export { AutoAppendTokenInterceptor };
//# sourceMappingURL=auto-append-token.interceptor.js.map