// core services
import { NgModule, Component } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CheckoutWrapperComponent } from './wrapper-components/checkout-wrapper.component';
import { CartWrapperComponent } from './wrapper-components/cart-wrapper.component';
import { HasTokenGuard } from './interceptors/has-token/has-token.guard';
import { BaseResolve } from './resolves/base.resolve';
import { FeaturedProductsResolver } from './resolves/features-products.resolve';
import { HomeWrapperComponent } from './wrapper-components/home-wrapper.component';
import { RegisterWrapperComponent } from './wrapper-components/register-wrapper.component';
import { ForgotPasswordWrapperComponent } from './wrapper-components/forgot-password-wrapper.component';
import { ResetPasswordWrapperComponent } from './wrapper-components/reset-password-wrapper.component';
import { ProfileWrapperComponent } from './wrapper-components/profile-wrapper.component';
import { IsProfiledUserGuard } from './interceptors/is-profiled-user/is-profiled-user.guard';
import { MeChangePasswordWrapperComponent } from './wrapper-components/me-change-password-wrapper.component';
import { AddressListWrapperComponent } from './wrapper-components/address-list-wrapper.component';
import { MeListBuyerAddressResolver } from './resolves/me.resolve';
import { PaymentListWrapperComponent } from './wrapper-components/payment-list-wrapper.component';
import { ProductListWrapperComponent } from './wrapper-components/product-list-wrapper.component';
import { MeListProductResolver, MeListCategoriesResolver,
  MeProductResolver, MeListSpecsResolver, MeListRelatedProductsResolver } from './resolves/me.product.resolve';
import { ProductDetailWrapperComponent } from './wrapper-components/product-detail-wrapper.component';
import { LoginWrapperComponent } from './wrapper-components/login-wrapper.component';
import { OrderDetailWrapperComponent } from './wrapper-components/order-detail-wrapper.component';

// auth components

// TODO - move or remove these
@Component({
  template: '<order-history [approvalVersion]="false"></order-history>',
})
export class MyOrdersComponent {}

@Component({
  template: '<order-history [approvalVersion]="true"></order-history>',
})
export class OrdersToApproveComponent {}

export const MarketplaceRoutes: Routes = [
  { path: 'login', component: LoginWrapperComponent },
  { path: 'register', component: RegisterWrapperComponent },
  { path: 'forgot-password', component: ForgotPasswordWrapperComponent },
  { path: 'reset-password', component: ResetPasswordWrapperComponent },
  { path: '',
    canActivate: [HasTokenGuard],
    resolve: {
      baseResolve: BaseResolve,
    },
    children : [
      { path: '', redirectTo: '/home', pathMatch: 'full' },
      { path: 'home', resolve: { featuredProducts: FeaturedProductsResolver }, component: HomeWrapperComponent },
      // { path: 'support', component: SupportComponent },
      // { path: 'faq', component: FaqComponent },
      // { path: 'terms-and-conditions', component: TermsAndConditionsComponent },
      { path: 'impersonation', redirectTo: '/home' },

      { path: 'checkout', component: CheckoutWrapperComponent },
      { path: 'cart', component: CartWrapperComponent },
      {
        path: 'products',
        component: ProductListWrapperComponent,
        resolve: {
          products: MeListProductResolver,
          categories: MeListCategoriesResolver,
        },
      },
      {
        path: 'products/:productID',
        resolve: {
          product: MeProductResolver,
          specList: MeListSpecsResolver,
        },
        children: [
          {
            path: '',
            component: ProductDetailWrapperComponent,
            resolve: {
              specs: MeListSpecsResolver,
              relatedProducts: MeListRelatedProductsResolver,
            },
          },
        ],
      },
      { path: 'profile' , canActivate: [IsProfiledUserGuard], children: [
        { path: '', component: ProfileWrapperComponent,  },
        { path: 'change-password', component: MeChangePasswordWrapperComponent },
        { path: 'addresses', component: AddressListWrapperComponent,
          resolve: {
            addresses: MeListBuyerAddressResolver,
          },
        },
        { path: 'payment-methods', component: PaymentListWrapperComponent },
        { path: 'orders', component: MyOrdersComponent },
        { path: 'orders/approval', component: OrdersToApproveComponent },
        { path: 'orders/:orderID', component: OrderDetailWrapperComponent },
        { path: 'orders/approval/:orderID', component: OrderDetailWrapperComponent },
      ], }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(MarketplaceRoutes)],
  exports: [RouterModule],
})
export class MarketplaceRoutingModule {}
