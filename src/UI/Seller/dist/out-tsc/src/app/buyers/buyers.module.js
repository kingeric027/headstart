import { __decorate } from "tslib";
import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';
import { BuyersRoutingModule } from './buyers-routing.module';
import { BuyerTableComponent } from './components/buyers/buyer-table/buyer-table.component';
import { BuyerUserTableComponent } from './components/users/buyer-user-table/buyer-user-table.component';
import { BuyerLocationTableComponent } from './components/locations/buyer-location-table/buyer-location-table.component';
import { BuyerPaymentTableComponent } from './components/payments/buyer-payment-table/buyer-payment-table.component';
import { BuyerApprovalTableComponent } from './components/approvals/buyer-approval-table/buyer-approval-table.component';
var BuyersModule = /** @class */ (function () {
    function BuyersModule() {
    }
    BuyersModule = __decorate([
        NgModule({
            imports: [SharedModule, BuyersRoutingModule],
            declarations: [
                BuyerTableComponent,
                BuyerApprovalTableComponent,
                BuyerLocationTableComponent,
                BuyerPaymentTableComponent,
                BuyerUserTableComponent,
            ],
        })
    ], BuyersModule);
    return BuyersModule;
}());
export { BuyersModule };
//# sourceMappingURL=buyers.module.js.map