import { __decorate } from "tslib";
import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { PromotionsRoutingModule } from './promotions-routing.module';
import { PromotionTableComponent } from './components/promotions/promotion-table/promotion-table.component';
var PromotionsModule = /** @class */ (function () {
    function PromotionsModule() {
    }
    PromotionsModule = __decorate([
        NgModule({
            imports: [SharedModule, PromotionsRoutingModule, PerfectScrollbarModule],
            declarations: [PromotionTableComponent],
        })
    ], PromotionsModule);
    return PromotionsModule;
}());
export { PromotionsModule };
//# sourceMappingURL=promotions.module.js.map