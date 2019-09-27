// components
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FaqComponent } from './static-pages/faq/faq.component';
import { SupportComponent } from './static-pages/support/support.component';

import { BaseResolve, IsProfiledUserGuard as isProfiledUser, HasTokenGuard as HasToken } from 'src/app/shared';
import { TermsAndConditionsComponent } from 'src/app/static-pages/terms-and-conditions/terms-and-conditions.component';
import { FeaturedProductsResolver } from './layout/resolves/features-products.resolve';
import { HomePageWrapperComponent } from './layout/home-wrapper/home-wrapper.component';
import { ProductsModule } from './product/product.module';
import { ProfileModule } from './profile/profile.module';
import { CheckoutModule } from './checkout/checkout.module';
import { OrderModule } from './order/order.module';

export function loadProductsModule() {
  return ProductsModule;
}

export function loadProfileModule() {
  return ProfileModule;
}

export function loadCheckoutModule() {
  return CheckoutModule;
}

export function loadOrderModule() {
  return OrderModule;
}

const routes: Routes = [
  {
    path: '',
    canActivate: [HasToken],
    resolve: {
      baseResolve: BaseResolve,
    },
    children: [
      { path: '', redirectTo: '/home', pathMatch: 'full' },
      { path: 'home', resolve: { featuredProducts: FeaturedProductsResolver }, component: HomePageWrapperComponent },
      {
        path: 'profile',
        loadChildren: './profile/profile.module#ProfileModule',
        canActivate: [isProfiledUser],
      },
      { path: 'support', component: SupportComponent },
      { path: 'faq', component: FaqComponent },
      { path: 'terms-and-conditions', component: TermsAndConditionsComponent },
      { path: 'products', loadChildren: './product/product.module#ProductsModule' },
      { path: '', loadChildren: './checkout/checkout.module#CheckoutModule' },
      { path: 'impersonation', redirectTo: '/home' },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
