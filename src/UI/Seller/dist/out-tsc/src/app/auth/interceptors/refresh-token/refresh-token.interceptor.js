import { __decorate, __metadata } from "tslib";
import { Injectable } from '@angular/core';
import { HttpErrorResponse, } from '@angular/common/http';
import { throwError } from 'rxjs';
import { catchError, filter, flatMap } from 'rxjs/operators';
import { AppAuthService } from '@app-seller/auth/services/app-auth.service';
/**
 * handle 401 unauthorized responses gracefully
 * by attempting to refresh token
 */
var RefreshTokenInterceptor = /** @class */ (function () {
    function RefreshTokenInterceptor(appAuthService) {
        this.appAuthService = appAuthService;
    }
    RefreshTokenInterceptor.prototype.intercept = function (request, next) {
        var _this = this;
        return next.handle(request).pipe(catchError(function (error) {
            // rethrow any non auth errors
            if (!_this.isAuthError(error)) {
                return throwError(error);
            }
            else {
                // if a refresh attempt failed recently then ignore (3 seconds)`
                if (_this.appAuthService.failedRefreshAttempt) {
                    return;
                }
                // ensure there is no outstanding request already fetching token
                // if there is then wait for the token to resolve
                var refreshToken = _this.appAuthService.refreshToken.getValue();
                if (refreshToken || _this.appAuthService.fetchingRefreshToken) {
                    return _this.appAuthService.refreshToken.pipe(filter(function (token) { return token !== ''; }), flatMap(function (token) {
                        request = request.clone({
                            setHeaders: { Authorization: "Bearer " + token },
                        });
                        return next.handle(request);
                    }));
                }
                else {
                    // attempt refresh for new token
                    return _this.appAuthService.refresh().pipe(flatMap(function (token) {
                        request = request.clone({
                            setHeaders: { Authorization: "Bearer " + token },
                        });
                        return next.handle(request);
                    }));
                }
            }
        }));
    };
    RefreshTokenInterceptor.prototype.isAuthError = function (error) {
        return (error instanceof HttpErrorResponse &&
            error.url.indexOf('ordercloud.io') > -1 &&
            error.status === 401);
    };
    RefreshTokenInterceptor = __decorate([
        Injectable({
            providedIn: 'root',
        }),
        __metadata("design:paramtypes", [AppAuthService])
    ], RefreshTokenInterceptor);
    return RefreshTokenInterceptor;
}());
export { RefreshTokenInterceptor };
//# sourceMappingURL=refresh-token.interceptor.js.map