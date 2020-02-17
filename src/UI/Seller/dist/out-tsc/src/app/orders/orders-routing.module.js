import { __decorate } from "tslib";
// core services
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { OrderTableComponent } from './components/order-table/order-table.component';
var routes = [
    { path: '', component: OrderTableComponent },
    { path: ':orderID', component: OrderTableComponent },
];
var OrdersRoutingModule = /** @class */ (function () {
    function OrdersRoutingModule() {
    }
    OrdersRoutingModule = __decorate([
        NgModule({
            imports: [RouterModule.forChild(routes)],
            exports: [RouterModule],
        })
    ], OrdersRoutingModule);
    return OrdersRoutingModule;
}());
export { OrdersRoutingModule };
//# sourceMappingURL=orders-routing.module.js.map