import { __decorate } from "tslib";
// components
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { HasTokenGuard as HasToken } from '@app-seller/shared';
import { HomeComponent } from '@app-seller/layout/home/home.component';
var routes = [
    {
        path: '',
        canActivate: [HasToken],
        children: [
            { path: '', redirectTo: '/home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            {
                path: 'products',
                loadChildren: function () { return import('./products/products.module').then(function (m) { return m.ProductsModule; }); },
            },
            {
                path: 'promotions',
                loadChildren: function () { return import('./promotions/promotions.module').then(function (m) { return m.PromotionsModule; }); },
            },
            {
                path: 'categories',
                loadChildren: function () { return import('./categories/categories.module').then(function (m) { return m.CategoriesModule; }); },
            },
            {
                path: 'orders',
                loadChildren: function () { return import('./orders/orders.module').then(function (m) { return m.OrdersModule; }); },
            },
            {
                path: 'buyers',
                loadChildren: function () { return import('./buyers/buyers.module').then(function (m) { return m.BuyersModule; }); },
            },
            {
                path: 'seller-users',
                loadChildren: function () { return import('./seller-users/seller-users.module').then(function (m) { return m.SellerUsersModule; }); },
            },
            {
                path: 'suppliers',
                loadChildren: function () { return import('./suppliers/suppliers.module').then(function (m) { return m.SuppliersModule; }); },
            },
            {
                path: 'my-supplier-profile',
                loadChildren: function () { return import('./suppliers/suppliers.module').then(function (m) { return m.SuppliersModule; }); },
            },
            {
                path: 'reports',
                loadChildren: function () { return import('./reports/reports.module').then(function (m) { return m.ReportsModule; }); },
            },
            {
                path: 'storefronts',
                loadChildren: function () { return import('./storefronts/storefronts.module').then(function (m) { return m.StorefrontsModule; }); },
            },
        ],
    },
];
var AppRoutingModule = /** @class */ (function () {
    function AppRoutingModule() {
    }
    AppRoutingModule = __decorate([
        NgModule({
            imports: [RouterModule.forRoot(routes)],
            exports: [RouterModule],
        })
    ], AppRoutingModule);
    return AppRoutingModule;
}());
export { AppRoutingModule };
//# sourceMappingURL=app-routing.module.js.map