import { __decorate } from "tslib";
// core services
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { BuyerTableComponent } from './components/buyers/buyer-table/buyer-table.component';
import { BuyerUserTableComponent } from './components/users/buyer-user-table/buyer-user-table.component';
import { BuyerLocationTableComponent } from './components/locations/buyer-location-table/buyer-location-table.component';
import { BuyerPaymentTableComponent } from './components/payments/buyer-payment-table/buyer-payment-table.component';
import { BuyerApprovalTableComponent } from './components/approvals/buyer-approval-table/buyer-approval-table.component';
var routes = [
    { path: '', component: BuyerTableComponent },
    { path: 'new', component: BuyerTableComponent },
    { path: ':buyerID', component: BuyerTableComponent },
    { path: ':buyerID/users', component: BuyerUserTableComponent },
    { path: ':buyerID/users/new', component: BuyerUserTableComponent },
    { path: ':buyerID/users/:userID', component: BuyerUserTableComponent },
    { path: ':buyerID/locations', component: BuyerLocationTableComponent },
    { path: ':buyerID/locations/new', component: BuyerLocationTableComponent },
    {
        path: ':buyerID/locations/:locationID',
        component: BuyerLocationTableComponent,
    },
    { path: ':buyerID/payments', component: BuyerPaymentTableComponent },
    { path: ':buyerID/payments/new', component: BuyerPaymentTableComponent },
    {
        path: ':buyerID/payments/:paymentID',
        component: BuyerPaymentTableComponent,
    },
    { path: ':buyerID/approvals', component: BuyerApprovalTableComponent },
    { path: ':buyerID/approvals/new', component: BuyerApprovalTableComponent },
    {
        path: ':buyerID/approvals/:approvalID',
        component: BuyerApprovalTableComponent,
    },
];
var BuyersRoutingModule = /** @class */ (function () {
    function BuyersRoutingModule() {
    }
    BuyersRoutingModule = __decorate([
        NgModule({
            imports: [RouterModule.forChild(routes)],
            exports: [RouterModule],
        })
    ], BuyersRoutingModule);
    return BuyersRoutingModule;
}());
export { BuyersRoutingModule };
//# sourceMappingURL=buyers-routing.module.js.map