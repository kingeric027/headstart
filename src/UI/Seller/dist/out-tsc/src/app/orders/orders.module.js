import { __decorate } from "tslib";
import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';
import { OrdersRoutingModule } from './orders-routing.module';
import { OrderTableComponent } from './components/order-table/order-table.component';
import { OrderDetailsComponent } from './components/order-details/order-details.component';
var OrdersModule = /** @class */ (function () {
    function OrdersModule() {
    }
    OrdersModule = __decorate([
        NgModule({
            imports: [SharedModule, OrdersRoutingModule],
            declarations: [OrderTableComponent, OrderDetailsComponent],
        })
    ], OrdersModule);
    return OrdersModule;
}());
export { OrdersModule };
//# sourceMappingURL=orders.module.js.map