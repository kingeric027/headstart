import { __decorate, __metadata } from "tslib";
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import * as jwtDecode from 'jwt-decode';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { OcTokenService } from '@ordercloud/angular-sdk';
import { AppStateService } from '@app-seller/shared/services/app-state/app-state.service';
import { AppAuthService } from '@app-seller/auth/services/app-auth.service';
var HasTokenGuard = /** @class */ (function () {
    function HasTokenGuard(ocTokenService, router, appAuthService, appStateService) {
        this.ocTokenService = ocTokenService;
        this.router = router;
        this.appAuthService = appAuthService;
        this.appStateService = appStateService;
    }
    HasTokenGuard.prototype.canActivate = function () {
        /**
         * very simple test to make sure a token exists,
         * can be parsed and has a valid expiration time
         *
         * shouldn't be depended on for actual token validation.
         * if the token is actually not valid it will fail on a call
         * and the refresh-token interceptor will handle it correctly
         */
        var isAccessTokenValid = this.isTokenValid();
        var refreshTokenExists = this.ocTokenService.GetRefresh() &&
            this.appAuthService.getRememberStatus();
        if (!isAccessTokenValid && refreshTokenExists) {
            return this.appAuthService.refresh().pipe(map(function () { return true; }));
        }
        if (!isAccessTokenValid) {
            this.router.navigate(['/login']);
        }
        this.appStateService.isLoggedIn.next(isAccessTokenValid);
        return of(isAccessTokenValid);
    };
    HasTokenGuard.prototype.isTokenValid = function () {
        var token = this.ocTokenService.GetAccess();
        if (!token) {
            return false;
        }
        var decodedToken;
        try {
            decodedToken = jwtDecode(token);
        }
        catch (e) {
            decodedToken = null;
        }
        if (!decodedToken) {
            return false;
        }
        var expiresIn = decodedToken.exp * 1000;
        return Date.now() < expiresIn;
    };
    HasTokenGuard = __decorate([
        Injectable({
            providedIn: 'root',
        }),
        __metadata("design:paramtypes", [OcTokenService,
            Router,
            AppAuthService,
            AppStateService])
    ], HasTokenGuard);
    return HasTokenGuard;
}());
export { HasTokenGuard };
//# sourceMappingURL=has-token.guard.js.map