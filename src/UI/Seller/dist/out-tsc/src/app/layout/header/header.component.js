import { __awaiter, __decorate, __generator, __metadata, __param } from "tslib";
import { Component, Inject } from '@angular/core';
import { applicationConfiguration } from '@app-seller/config/app.config';
import { faBoxOpen, faSignOutAlt, faUser, faUsers, faMapMarkerAlt, faSitemap, faUserCircle, } from '@fortawesome/free-solid-svg-icons';
import { OcTokenService } from '@ordercloud/angular-sdk';
import { Router, NavigationEnd } from '@angular/router';
import { AppStateService } from '@app-seller/shared';
import { getHeaderConfig } from './header.config';
import { AppAuthService } from '@app-seller/auth';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
var HeaderComponent = /** @class */ (function () {
    function HeaderComponent(ocTokenService, router, appStateService, appAuthService, currentUserService, appConfig) {
        var _this = this;
        this.ocTokenService = ocTokenService;
        this.router = router;
        this.appStateService = appStateService;
        this.appAuthService = appAuthService;
        this.currentUserService = currentUserService;
        this.appConfig = appConfig;
        this.isCollapsed = true;
        this.faBoxOpen = faBoxOpen;
        this.faUser = faUser;
        this.faSignOutAlt = faSignOutAlt;
        this.faUsers = faUsers;
        this.faMapMarker = faMapMarkerAlt;
        this.faSitemap = faSitemap;
        this.faUserCircle = faUserCircle;
        this.activeTitle = '';
        this.urlChange = function (url) {
            var activeNavGroup = _this.headerConfig.find(function (grouping) {
                return url.includes(grouping.route);
            });
            _this.activeTitle = activeNavGroup && activeNavGroup.title;
        };
    }
    HeaderComponent.prototype.ngOnInit = function () {
        this.headerConfig = getHeaderConfig(this.appAuthService.getUserRoles(), this.appAuthService.getOrdercloudUserType());
        this.getCurrentUser();
        this.subscribeToRouteEvents();
        this.urlChange(this.router.url);
    };
    HeaderComponent.prototype.getCurrentUser = function () {
        return __awaiter(this, void 0, void 0, function () {
            var _a, _b;
            return __generator(this, function (_c) {
                switch (_c.label) {
                    case 0:
                        _a = this;
                        return [4 /*yield*/, this.currentUserService.getUser()];
                    case 1:
                        _a.user = _c.sent();
                        _b = this;
                        return [4 /*yield*/, this.currentUserService.isSupplierUser()];
                    case 2:
                        _b.isSupplierUser = _c.sent();
                        this.isSupplierUser ? this.getSupplierOrg() : (this.organizationName = this.appConfig.sellerName);
                        return [2 /*return*/];
                }
            });
        });
    };
    HeaderComponent.prototype.getSupplierOrg = function () {
        return __awaiter(this, void 0, void 0, function () {
            var _a;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        _a = this;
                        return [4 /*yield*/, this.currentUserService.getSupplierOrg()];
                    case 1:
                        _a.organizationName = _b.sent();
                        return [2 /*return*/];
                }
            });
        });
    };
    HeaderComponent.prototype.subscribeToRouteEvents = function () {
        var _this = this;
        this.router.events.subscribe(function (ev) {
            if (ev instanceof NavigationEnd) {
                _this.urlChange(ev.url);
            }
        });
    };
    HeaderComponent.prototype.logout = function () {
        this.ocTokenService.RemoveAccess();
        this.appStateService.isLoggedIn.next(false);
        this.router.navigate(['/login']);
    };
    HeaderComponent = __decorate([
        Component({
            selector: 'layout-header',
            templateUrl: './header.component.html',
            styleUrls: ['./header.component.scss'],
        }),
        __param(5, Inject(applicationConfiguration)),
        __metadata("design:paramtypes", [OcTokenService,
            Router,
            AppStateService,
            AppAuthService,
            CurrentUserService, Object])
    ], HeaderComponent);
    return HeaderComponent;
}());
export { HeaderComponent };
//# sourceMappingURL=header.component.js.map