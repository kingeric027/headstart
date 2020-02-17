import { __awaiter, __decorate, __generator, __metadata, __param } from "tslib";
import { Injectable, Inject } from '@angular/core';
import { OcMeService, OcAuthService, OcTokenService, OcSupplierService } from '@ordercloud/angular-sdk';
import { applicationConfiguration } from '@app-seller/config/app.config';
import { AppAuthService } from '@app-seller/auth/services/app-auth.service';
import { AppStateService } from '../app-state/app-state.service';
var CurrentUserService = /** @class */ (function () {
    function CurrentUserService(ocMeService, ocAuthService, appConfig, ocTokenService, appAuthService, appStateService, ocSupplierService) {
        this.ocMeService = ocMeService;
        this.ocAuthService = ocAuthService;
        this.appConfig = appConfig;
        this.ocTokenService = ocTokenService;
        this.appAuthService = appAuthService;
        this.appStateService = appStateService;
        this.ocSupplierService = ocSupplierService;
    }
    CurrentUserService.prototype.login = function (username, password, rememberMe) {
        return __awaiter(this, void 0, void 0, function () {
            var accessToken, _a;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: return [4 /*yield*/, this.ocAuthService
                            .Login(username, password, this.appConfig.clientID, this.appConfig.scope)
                            .toPromise()];
                    case 1:
                        accessToken = _b.sent();
                        if (rememberMe && accessToken.refresh_token) {
                            /**
                             * set the token duration in the dashboard - https://developer.ordercloud.io/dashboard/settings
                             * refresh tokens are configured per clientID and initially set to 0
                             * a refresh token of 0 means no refresh token is returned in OAuth accessToken
                             */
                            this.ocTokenService.SetRefresh(accessToken.refresh_token);
                            this.appAuthService.setRememberStatus(true);
                        }
                        this.ocTokenService.SetAccess(accessToken.access_token);
                        this.appStateService.isLoggedIn.next(true);
                        _a = this;
                        return [4 /*yield*/, this.ocMeService.Get().toPromise()];
                    case 2:
                        _a.me = _b.sent();
                        return [2 /*return*/];
                }
            });
        });
    };
    CurrentUserService.prototype.getUser = function () {
        return __awaiter(this, void 0, void 0, function () {
            var _a;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        if (!this.me) return [3 /*break*/, 1];
                        _a = this.me;
                        return [3 /*break*/, 3];
                    case 1: return [4 /*yield*/, this.ocMeService.Get().toPromise()];
                    case 2:
                        _a = _b.sent();
                        _b.label = 3;
                    case 3: return [2 /*return*/, _a];
                }
            });
        });
    };
    CurrentUserService.prototype.getUserContext = function () {
        return __awaiter(this, void 0, void 0, function () {
            var UserContext;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, this.constructUserContext()];
                    case 1:
                        UserContext = _a.sent();
                        return [2 /*return*/, UserContext];
                }
            });
        });
    };
    CurrentUserService.prototype.constructUserContext = function () {
        return __awaiter(this, void 0, void 0, function () {
            var me, userType, userRoles;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, this.getUser()];
                    case 1:
                        me = _a.sent();
                        return [4 /*yield*/, this.appAuthService.getOrdercloudUserType()];
                    case 2:
                        userType = _a.sent();
                        return [4 /*yield*/, this.appAuthService.getUserRoles()];
                    case 3:
                        userRoles = _a.sent();
                        return [2 /*return*/, {
                                Me: me,
                                UserType: userType,
                                UserRoles: userRoles,
                            }];
                }
            });
        });
    };
    CurrentUserService.prototype.isSupplierUser = function () {
        return __awaiter(this, void 0, void 0, function () {
            var me;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, this.getUser()];
                    case 1:
                        me = _a.sent();
                        return [2 /*return*/, me.Supplier ? true : false];
                }
            });
        });
    };
    CurrentUserService.prototype.getSupplierOrg = function () {
        return __awaiter(this, void 0, void 0, function () {
            var me, supplier;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, this.getUser()];
                    case 1:
                        me = _a.sent();
                        return [4 /*yield*/, this.ocSupplierService.Get(me.Supplier.ID).toPromise()];
                    case 2:
                        supplier = _a.sent();
                        return [2 /*return*/, supplier.Name];
                }
            });
        });
    };
    CurrentUserService = __decorate([
        Injectable({
            providedIn: 'root',
        }),
        __param(2, Inject(applicationConfiguration)),
        __metadata("design:paramtypes", [OcMeService,
            OcAuthService, Object, OcTokenService,
            AppAuthService,
            AppStateService,
            OcSupplierService])
    ], CurrentUserService);
    return CurrentUserService;
}());
export { CurrentUserService };
//# sourceMappingURL=current-user.service.js.map