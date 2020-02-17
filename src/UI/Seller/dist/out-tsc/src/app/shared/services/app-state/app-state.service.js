import { __decorate, __metadata } from "tslib";
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
var AppStateService = /** @class */ (function () {
    function AppStateService() {
        // This will not control access to routes or API features, it is purely for display rules.
        this.isLoggedIn = new BehaviorSubject(false);
    }
    AppStateService = __decorate([
        Injectable({
            providedIn: 'root',
        }),
        __metadata("design:paramtypes", [])
    ], AppStateService);
    return AppStateService;
}());
export { AppStateService };
//# sourceMappingURL=app-state.service.js.map