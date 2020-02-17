import { __decorate } from "tslib";
// core services
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { StorefrontListComponent } from './components/storefront-list/storefront-list.component';
var routes = [{ path: '', component: StorefrontListComponent }];
var StorefrontsRoutingModule = /** @class */ (function () {
    function StorefrontsRoutingModule() {
    }
    StorefrontsRoutingModule = __decorate([
        NgModule({
            imports: [RouterModule.forChild(routes)],
            exports: [RouterModule],
        })
    ], StorefrontsRoutingModule);
    return StorefrontsRoutingModule;
}());
export { StorefrontsRoutingModule };
//# sourceMappingURL=storefronts-routing.module.js.map