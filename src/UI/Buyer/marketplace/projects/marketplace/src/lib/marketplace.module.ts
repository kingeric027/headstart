// core services
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { MarketplaceRoutingModule } from './marketplace-routing.module';
import { CartWrapperComponent } from './wrapper-components/cart-wrapper.component';
import { CheckoutWrapperComponent } from './wrapper-components/checkout-wrapper.component';
import { AddressListWrapperComponent } from './wrapper-components/address-list-wrapper.component';
import { LocationListWrapperComponent } from './wrapper-components/location-list-wrapper.component';
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
import { MeListAddressResolver, MeListBuyerLocationResolver } from './resolves/me.resolve';
import { MeProductResolver } from './resolves/me.product.resolve';
import { AuthService } from './services/auth/auth.service';
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
import { CreditCardService } from './services/current-user/credit-card.service';
import { CurrentOrderService } from './services/order/order.service';
import { CartService } from './services/order/cart.service';
import { CheckoutService } from './services/order/checkout.service';
import { OrderStateService } from './services/order/order-state.service';
import { Configuration as MktpConfiguration } from '@ordercloud/headstart-sdk';
import { Configuration, SdkConfiguration } from 'ordercloud-javascript-sdk';
import { AppConfig } from './shopper-context';
import { OrdersToApproveStateService } from './services/order-history/order-to-approve-state.service';
import { LocationManagementWrapperComponent } from './wrapper-components/location-management-wrapper.component';
import { ExchangeRatesService } from './services/exchange-rates/exchange-rates.service';
import { OrderHistoryWrapperComponent } from './wrapper-components/order-history-wrapper-component';
import { OrdercloudEnv } from './shopper-context';
import { TempSdk } from './services/temp-sdk/temp-sdk.service';
import { StaticPageWrapperComponent } from './wrapper-components/static-page-wrapper.component';

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [MarketplaceRoutingModule, CommonModule],
  providers: [
    MeListAddressResolver,
    MeListBuyerLocationResolver,
    MeProductResolver,
    AuthService,
    CreditCardService,
    CurrentOrderService,
    CurrentUserService,
    ExchangeRatesService,
    OrderHistoryService,
    OrdersToApproveStateService,
    TempSdk,
    PaymentHelperService,
    ProductFilterService,
    ReorderHelperService,
    RouteService,
    TokenHelperService,
    CartService,
    CheckoutService,
    OrderStateService,
    ShopperContextService,
  ],
  declarations: [
    CartWrapperComponent,
    CheckoutWrapperComponent,
    AddressListWrapperComponent,
    LocationListWrapperComponent,
    LocationManagementWrapperComponent,
    ForgotPasswordWrapperComponent,
    HomeWrapperComponent,
    StaticPageWrapperComponent,
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
    OrderHistoryWrapperComponent,
    SupplierListWrapperComponent,
  ],
})
export class MarketplaceModule {
  constructor(private appConfig: AppConfig) {
    MktpConfiguration.Set({
      baseApiUrl: this.appConfig.middlewareUrl,
    });
    Configuration.Set(this.getOrdercloudSDKConfig(appConfig));
  }

  private getOrdercloudSDKConfig(config: AppConfig): SdkConfiguration {
    const apiUrl = this.getApiUrl(config.ordercloudEnv);
    return {
      baseApiUrl: `${apiUrl}`,
      clientID: config.clientID,
      cookieOptions: { prefix: config.appname.replace(/ /g, '_').toLowerCase() },
    };
  }

  private getApiUrl(env: OrdercloudEnv): string {
    switch (env) {
      case OrdercloudEnv.Sandbox:
        return `https://sandboxapi.ordercloud.io`;
      case OrdercloudEnv.Staging:
        return `https://stagingapi.ordercloud.io`;
      case OrdercloudEnv.Production:
        return `https://api.ordercloud.io`;
    }
  }
}
