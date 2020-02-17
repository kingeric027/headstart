import { __decorate } from "tslib";
// core services
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CategoryListComponent } from './components/categories/category-list/category-list.component';
import { CategoryCreateComponent } from './components/categories/category-create/category-create.component';
import { CategoryDetailsComponent } from '@app-seller/shared/components/category-details/category-details.component';
var routes = [
    { path: '', component: CategoryListComponent },
    { path: 'new', component: CategoryCreateComponent },
    { path: ':categoryID', component: CategoryDetailsComponent },
];
var CategoriesRoutingModule = /** @class */ (function () {
    function CategoriesRoutingModule() {
    }
    CategoriesRoutingModule = __decorate([
        NgModule({
            imports: [RouterModule.forChild(routes)],
            exports: [RouterModule],
        })
    ], CategoriesRoutingModule);
    return CategoriesRoutingModule;
}());
export { CategoriesRoutingModule };
//# sourceMappingURL=categories-routing.module.js.map