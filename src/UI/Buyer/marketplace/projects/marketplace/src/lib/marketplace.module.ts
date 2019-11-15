// core services
import { NgModule, CUSTOM_ELEMENTS_SCHEMA, Component } from '@angular/core';
import { MarketplaceRoutingModule, MyOrdersWrapperComponent, OrdersToApproveWrapperComponent } from './marketplace-routing.module';
import { CartWrapperComponent } from './wrapper-components/cart-wrapper.component';
import { CheckoutWrapperComponent } from './wrapper-components/checkout-wrapper.component';
import { AddressListWrapperComponent } from './wrapper-components/address-list-wrapper.component';
import { ForgotPasswordWrapperComponent } from './wrapper-components/forgot-password-wrapper.component';
import { HomeWrapperComponent } from './wrapper-components/home-wrapper.component';
import { LoginWrapperComponent } from './wrapper-components/login-wrapper.component';
import { MeChangePasswordWrapperComponent } from './wrapper-components/me-change-password-wrapper.component';
import { PaymentListWrapperComponent } from './wrapper-components/payment-list-wrapper.component';
import { ProductDetailWrapperComponent } from './wrapper-components/product-detail-wrapper.component';
import { ProductListWrapperComponent } from './wrapper-components/product-list-wrapper.component';
import { ProfileWrapperComponent } from './wrapper-components/profile-wrapper.component';
import { RegisterWrapperComponent } from './wrapper-components/register-wrapper.component';
import { ResetPasswordWrapperComponent } from './wrapper-components/reset-password-wrapper.component';
import { FeaturedProductsResolver } from './resolves/features-products.resolve';
import { MeListBuyerAddressResolver } from './resolves/me.resolve';
import { MeListCategoriesResolver, MeProductResolver, MeListSpecsResolver, MeListRelatedProductsResolver } from './resolves/me.product.resolve';
import { AuthNetCreditCardService } from './services/authorize-net/authorize-net.service';
import { AuthService } from './services/auth/auth.service';
import { CurrentOrderService } from './services/current-order/current-order.service';
import { CurrentUserService } from './services/current-user/current-user.service';
import { OrderHistoryService } from './services/order-history/order-history.service';
import { PaymentHelperService } from './services/payment-helper/payment-helper.service';
import { ProductFilterService } from './services/product-filter/product-filter.service';
import { ReorderHelperService } from './services/reorder/reorder.service';
import { RouteService } from './services/route/route.service';
import { TokenHelperService } from './services/token-helper/token-helper.service';
import { ShopperContextService } from './services/shopper-context/shopper-context.service';
import { CommonModule } from '@angular/common';
import { OrderDetailWrapperComponent } from './wrapper-components/order-detail-wrapper.component';
import { OrderShipmentsWrapperComponent } from './wrapper-components/order-shipments-wrapper.component';
import { SupplierListWrapperComponent } from './wrapper-components/supplier-list-wrapper.component';


@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [
    MarketplaceRoutingModule,
    CommonModule,
  ],
  providers: [
    FeaturedProductsResolver,
    MeListBuyerAddressResolver,
    MeListCategoriesResolver,
    MeProductResolver,
    MeListSpecsResolver,
    MeListRelatedProductsResolver,

    AuthNetCreditCardService,
    AuthService,
    CurrentOrderService,
    CurrentUserService,
    OrderHistoryService,
    PaymentHelperService,
    ProductFilterService,
    ReorderHelperService,
    RouteService,
    TokenHelperService,
    ShopperContextService
  ],
  declarations: [
    CartWrapperComponent,
    CheckoutWrapperComponent,
    AddressListWrapperComponent,
    ForgotPasswordWrapperComponent,
    HomeWrapperComponent,
    LoginWrapperComponent,
    MeChangePasswordWrapperComponent,
    PaymentListWrapperComponent,
    ProductDetailWrapperComponent,
    ProductListWrapperComponent,
    ProfileWrapperComponent,
    RegisterWrapperComponent,
    ResetPasswordWrapperComponent,
    OrderDetailWrapperComponent,
    OrderShipmentsWrapperComponent,
    MyOrdersWrapperComponent,
    OrdersToApproveWrapperComponent,
    SupplierListWrapperComponent  
  ],
})
export class MarketplaceModule {}


