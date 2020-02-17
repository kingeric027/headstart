import { __decorate } from "tslib";
// core services
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { SupplierTableComponent } from './components/suppliers/supplier-table/supplier-table.component';
import { SupplierUserTableComponent } from './components/users/user-table/supplier-user-table.component';
import { SupplierLocationTableComponent } from './components/locations/supplier-location-table/supplier-location-table.component';
var routes = [
    { path: '', component: SupplierTableComponent },
    { path: 'new', component: SupplierTableComponent },
    { path: ':supplierID', component: SupplierTableComponent },
    { path: ':supplierID/users', component: SupplierUserTableComponent },
    { path: ':supplierID/users/new', component: SupplierUserTableComponent },
    { path: ':supplierID/users/:userID', component: SupplierUserTableComponent },
    { path: ':supplierID/locations', component: SupplierLocationTableComponent },
    { path: ':supplierID/locations/new', component: SupplierLocationTableComponent },
    {
        path: ':supplierID/locations/:locationID',
        component: SupplierLocationTableComponent,
    },
];
var SuppliersRoutingModule = /** @class */ (function () {
    function SuppliersRoutingModule() {
    }
    SuppliersRoutingModule = __decorate([
        NgModule({
            imports: [RouterModule.forChild(routes)],
            exports: [RouterModule],
        })
    ], SuppliersRoutingModule);
    return SuppliersRoutingModule;
}());
export { SuppliersRoutingModule };
//# sourceMappingURL=suppliers-routing.module.js.map