import { __decorate, __metadata } from "tslib";
import { Component } from '@angular/core';
import { AppStateService } from '@app-seller/shared';
var AppComponent = /** @class */ (function () {
    function AppComponent(appStateService) {
        this.appStateService = appStateService;
        this.isLoggedIn$ = this.appStateService.isLoggedIn;
    }
    AppComponent = __decorate([
        Component({
            selector: 'app-root',
            templateUrl: './app.component.html',
            styleUrls: ['./app.component.scss'],
        }),
        __metadata("design:paramtypes", [AppStateService])
    ], AppComponent);
    return AppComponent;
}());
export { AppComponent };
//# sourceMappingURL=app.component.js.map