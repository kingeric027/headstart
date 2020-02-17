import { __decorate } from "tslib";
import { NgModule } from '@angular/core';
import { HeaderComponent } from '@app-seller/layout/header/header.component';
import { HomeComponent } from '@app-seller/layout/home/home.component';
import { SharedModule } from '@app-seller/shared';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
var LayoutModule = /** @class */ (function () {
    function LayoutModule() {
    }
    LayoutModule = __decorate([
        NgModule({
            imports: [RouterModule, SharedModule, FormsModule],
            exports: [HeaderComponent, HomeComponent],
            declarations: [HeaderComponent, HomeComponent],
        })
    ], LayoutModule);
    return LayoutModule;
}());
export { LayoutModule };
//# sourceMappingURL=layout.module.js.map