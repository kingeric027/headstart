import { __decorate } from "tslib";
import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';
import { StorefrontListComponent } from './components/storefront-list/storefront-list.component';
import { StorefrontsRoutingModule } from './storefronts-routing.module';
var StorefrontsModule = /** @class */ (function () {
    function StorefrontsModule() {
    }
    StorefrontsModule = __decorate([
        NgModule({
            imports: [SharedModule, StorefrontsRoutingModule],
            declarations: [StorefrontListComponent],
        })
    ], StorefrontsModule);
    return StorefrontsModule;
}());
export { StorefrontsModule };
//# sourceMappingURL=storefronts.module.js.map