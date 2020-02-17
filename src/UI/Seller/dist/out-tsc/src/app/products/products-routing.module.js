import { __decorate } from "tslib";
// core services
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ProductTableComponent } from './product-table/product-table.component';
var routes = [
    { path: '', component: ProductTableComponent, pathMatch: 'prefix' },
    { path: 'new', component: ProductTableComponent, pathMatch: 'full' },
    { path: ':productID', component: ProductTableComponent, pathMatch: 'full' },
];
var ProductsRoutingModule = /** @class */ (function () {
    function ProductsRoutingModule() {
    }
    ProductsRoutingModule = __decorate([
        NgModule({
            imports: [RouterModule.forChild(routes)],
            exports: [RouterModule],
        })
    ], ProductsRoutingModule);
    return ProductsRoutingModule;
}());
export { ProductsRoutingModule };
//# sourceMappingURL=products-routing.module.js.map