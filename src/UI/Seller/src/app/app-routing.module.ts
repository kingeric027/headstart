// components
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HasTokenGuard as HasToken } from '@app-seller/shared';
import { HomeComponent } from '@app-seller/layout/home/home.component';

const routes: Routes = [
  {
    path: '',
    canActivate: [HasToken],
    children: [
      { path: '', redirectTo: '/home', pathMatch: 'full' },
      { path: 'home', component: HomeComponent },
      {
        path: 'products',
        loadChildren: () => import('./products/products.module').then((m) => m.ProductsModule),
      },
      {
        path: 'buyers',
        loadChildren: () => import('./buyers/buyers.module').then((m) => m.BuyersModule),
      },
      {
        path: 'suppliers',
        loadChildren: () => import('./suppliers/suppliers.module').then((m) => m.SuppliersModule),
      },
      {
        path: 'sellers',
        loadChildren: () => import('./sellers/sellers.module').then((m) => m.SellersModule),
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
