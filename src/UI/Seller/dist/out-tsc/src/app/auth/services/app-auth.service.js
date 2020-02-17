import { __decorate, __metadata, __param } from "tslib";
import { Injectable, Inject } from '@angular/core';
import { throwError, of, BehaviorSubject } from 'rxjs';
import { tap, catchError, finalize, map } from 'rxjs/operators';
import { Router } from '@angular/router';
// 3rd party
import { OcTokenService, OcAuthService } from '@ordercloud/angular-sdk';
import { applicationConfiguration } from '@app-seller/config/app.config';
import { CookieService } from 'ngx-cookie';
import { keys as _keys } from 'lodash';
import { AppStateService } from '@app-seller/shared/services/app-state/app-state.service';
import * as jwtDecode from 'jwt-decode';
import { SELLER, SUPPLIER } from '@app-seller/shared/models/ordercloud-user.types';
export var TokenRefreshAttemptNotPossible = 'Token refresh attempt not possible';
var AppAuthService = /** @class */ (function () {
    function AppAuthService(ocTokenService, ocAuthService, cookieService, router, appStateService, appConfig) {
        this.ocTokenService = ocTokenService;
        this.ocAuthService = ocAuthService;
        this.cookieService = cookieService;
        this.router = router;
        this.appStateService = appStateService;
        this.appConfig = appConfig;
        this.rememberMeCookieName = this.appConfig.appname.replace(/ /g, '_').toLowerCase() + "_rememberMe";
        this.fetchingRefreshToken = false;
        this.failedRefreshAttempt = false;
        this.refreshToken = new BehaviorSubject('');
    }
    AppAuthService.prototype.refresh = function () {
        var _this = this;
        this.fetchingRefreshToken = true;
        return this.fetchRefreshToken().pipe(tap(function (token) {
            _this.ocTokenService.SetAccess(token);
            _this.refreshToken.next(token);
            _this.appStateService.isLoggedIn.next(true);
        }), catchError(function () {
            // ignore new refresh attempts if a refresh
            // attempt failed within the last 3 seconds
            _this.failedRefreshAttempt = true;
            setTimeout(function () {
                _this.failedRefreshAttempt = false;
            }, 3000);
            return _this.logout();
        }), finalize(function () {
            _this.fetchingRefreshToken = false;
        }));
    };
    AppAuthService.prototype.getDecodedToken = function () {
        var userToken = this.ocTokenService.GetAccess();
        var decodedToken;
        try {
            decodedToken = jwtDecode(userToken);
        }
        catch (e) {
            decodedToken = null;
        }
        if (!decodedToken) {
            throw new Error('decoded jwt was null when attempting to get user roles');
        }
        return decodedToken;
    };
    AppAuthService.prototype.getUserRoles = function () {
        var roles = this.getRolesFromToken();
        return roles;
    };
    AppAuthService.prototype.getOrdercloudUserType = function () {
        var usrtype = this.getUsrTypeFromToken();
        var OrdercloudUserType = usrtype === 'admin' ? SELLER : SUPPLIER;
        return OrdercloudUserType;
    };
    AppAuthService.prototype.getRolesFromToken = function () {
        var decodedToken = this.getDecodedToken();
        return decodedToken.role;
    };
    AppAuthService.prototype.getUsrTypeFromToken = function () {
        var decodedToken = this.getDecodedToken();
        return decodedToken.usrtype;
    };
    AppAuthService.prototype.fetchToken = function () {
        var accessToken = this.ocTokenService.GetAccess();
        if (accessToken) {
            return of(accessToken);
        }
        return this.fetchRefreshToken();
    };
    AppAuthService.prototype.fetchRefreshToken = function () {
        var _this = this;
        var refreshToken = this.ocTokenService.GetRefresh();
        if (refreshToken) {
            return this.ocAuthService.RefreshToken(refreshToken, this.appConfig.clientID).pipe(map(function (authResponse) { return authResponse.access_token; }), tap(function (token) { return _this.ocTokenService.SetAccess(token); }), catchError(function (error) {
                return throwError(error);
            }));
        }
        throwError(TokenRefreshAttemptNotPossible);
    };
    AppAuthService.prototype.logout = function () {
        var _this = this;
        var cookiePrefix = this.appConfig.appname.replace(/ /g, '_').toLowerCase();
        var appCookieNames = _keys(this.cookieService.getAll());
        appCookieNames.forEach(function (cookieName) {
            if (cookieName.indexOf(cookiePrefix) > -1) {
                _this.cookieService.remove(cookieName);
            }
        });
        this.appStateService.isLoggedIn.next(false);
        return of(this.router.navigate(['/login']));
    };
    AppAuthService.prototype.setRememberStatus = function (status) {
        this.cookieService.putObject(this.rememberMeCookieName, { status: status });
    };
    AppAuthService.prototype.getRememberStatus = function () {
        var rememberMe = this.cookieService.getObject(this.rememberMeCookieName);
        return !!(rememberMe && rememberMe.status);
    };
    AppAuthService = __decorate([
        Injectable({
            providedIn: 'root',
        }),
        __param(5, Inject(applicationConfiguration)),
        __metadata("design:paramtypes", [OcTokenService,
            OcAuthService,
            CookieService,
            Router,
            AppStateService, Object])
    ], AppAuthService);
    return AppAuthService;
}());
export { AppAuthService };
//# sourceMappingURL=app-auth.service.js.map