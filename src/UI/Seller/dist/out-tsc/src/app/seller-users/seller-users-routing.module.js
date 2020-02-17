import { __decorate } from "tslib";
// core services
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { SellerUserTableComponent } from './components/seller-user-table/seller-user-table.component';
var routes = [
    { path: '', component: SellerUserTableComponent },
    { path: 'new', component: SellerUserTableComponent },
    { path: ':userID', component: SellerUserTableComponent },
];
var SellerUsersRoutingModule = /** @class */ (function () {
    function SellerUsersRoutingModule() {
    }
    SellerUsersRoutingModule = __decorate([
        NgModule({
            imports: [RouterModule.forChild(routes)],
            exports: [RouterModule],
        })
    ], SellerUsersRoutingModule);
    return SellerUsersRoutingModule;
}());
export { SellerUsersRoutingModule };
//# sourceMappingURL=seller-users-routing.module.js.map