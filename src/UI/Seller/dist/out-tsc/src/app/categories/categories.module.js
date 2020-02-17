import { __decorate } from "tslib";
import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';
import { CategoryListComponent } from './components/categories/category-list/category-list.component';
import { CategoryNewComponent } from './components/categories/category-new/category-new.component';
import { CategoryCreateComponent } from './components/categories/category-create/category-create.component';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { CategoriesRoutingModule } from './categories-routing.module';
var CategoriesModule = /** @class */ (function () {
    function CategoriesModule() {
    }
    CategoriesModule = __decorate([
        NgModule({
            imports: [SharedModule, CategoriesRoutingModule, PerfectScrollbarModule],
            declarations: [CategoryListComponent, CategoryNewComponent, CategoryCreateComponent],
        })
    ], CategoriesModule);
    return CategoriesModule;
}());
export { CategoriesModule };
//# sourceMappingURL=categories.module.js.map