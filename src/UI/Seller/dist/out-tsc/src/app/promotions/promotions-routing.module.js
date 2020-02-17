import { __decorate } from "tslib";
// core services
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { PromotionTableComponent } from './components/promotions/promotion-table/promotion-table.component';
var routes = [
    { path: '', component: PromotionTableComponent },
    { path: 'new', component: PromotionTableComponent },
    { path: ':promotionID', component: PromotionTableComponent },
];
var PromotionsRoutingModule = /** @class */ (function () {
    function PromotionsRoutingModule() {
    }
    PromotionsRoutingModule = __decorate([
        NgModule({
            imports: [RouterModule.forChild(routes)],
            exports: [RouterModule],
        })
    ], PromotionsRoutingModule);
    return PromotionsRoutingModule;
}());
export { PromotionsRoutingModule };
//# sourceMappingURL=promotions-routing.module.js.map