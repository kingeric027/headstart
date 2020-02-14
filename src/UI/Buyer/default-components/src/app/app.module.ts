import { BrowserModule } from '@angular/platform-browser';
import {
  NgModule,
  Injector,
  Inject,
  PLATFORM_ID,
  CUSTOM_ELEMENTS_SCHEMA,
  NO_ERRORS_SCHEMA,
  ErrorHandler,
} from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { MarketplaceModule, AppConfig } from 'marketplace';
import { createCustomElement } from '@angular/elements';
import { isPlatformBrowser, DatePipe } from '@angular/common';
import { CookieModule } from 'ngx-cookie';
import { OrderCloudModule } from '@ordercloud/angular-sdk';
import { ToastrModule } from 'ngx-toastr';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { NgxImageZoomModule } from 'ngx-image-zoom';
import { OCMCategoryDropdown } from './components/layout/category-dropdown/category-dropdown.component';
import {
  NgbCarouselModule,
  NgbTooltipModule,
  NgbCollapseModule,
  NgbPaginationModule,
  NgbPopoverModule,
  NgbDropdownModule,
  NgbDatepickerModule,
  NgbAccordionModule,
  NgbDateAdapter,
  NgbModule,
} from '@ng-bootstrap/ng-bootstrap';
import { FormControlErrorDirective } from './directives/form-control-errors.directive';
import { CreditCardInputDirective } from './directives/credit-card-input.directive';
import { ProductNameWithSpecsPipe } from './pipes/product-name-with-specs.pipe';
import { OrderStatusDisplayPipe } from './pipes/order-status-display.pipe';
import { PhoneFormatPipe } from './pipes/phone-format.pipe';
import { ChildCategoryPipe } from './pipes/category-children.pipe';
import { CreditCardFormatPipe } from './pipes/credit-card-format.pipe';
import { PaymentMethodDisplayPipe } from './pipes/payment-method-display.pipe';
import { HttpClientModule } from '@angular/common/http';
import { OcSDKConfig } from './config/ordercloud-sdk.config';
import { ComponentNgElementStrategyFactory } from 'src/lib/component-factory-strategy';
import { NgbDateNativeAdapter } from './config/date-picker.config';
import { AppErrorHandler } from './config/error-handling.config';
import { NgProgressModule } from '@ngx-progressbar/core';
import { NgProgressHttpModule } from '@ngx-progressbar/http';
import { OCMOrderApproval } from './components/orders/order-approval/order-approval.component';
import { OCMOrderShipments } from './components/orders/order-shipments/order-shipments.component';
import { ShipperTrackingPipe, ShipperTrackingSupportedPipe } from './pipes/shipperTracking.pipe';
import { OCMOrderHistorical } from './components/orders/order-historical/order-historical.component';
import { OCMOrderHistory } from './components/orders/order-history/order-history.component';

import { SpecFieldDirective } from './components/products/spec-form/spec-field.directive';
import { SpecFormCheckboxComponent } from './components/products/spec-form/spec-form-checkbox/spec-form-checkbox.component';
import { SpecFormInputComponent } from './components/products/spec-form/spec-form-input/spec-form-input.component';
import { SpecFormLabelComponent } from './components/products/spec-form/spec-form-label/spec-form-label.component';
import { SpecFormNumberComponent } from './components/products/spec-form/spec-form-number/spec-form-number.component';
import { SpecFormRangeComponent } from './components/products/spec-form/spec-form-range/spec-form-range.component';
import { SpecFormSelectComponent } from './components/products/spec-form/spec-form-select/spec-form-select.component';
import { SpecFormTextAreaComponent } from './components/products/spec-form/spec-form-textarea/spec-form-textarea.component';
import { OCMSupplierList } from './components/suppliers/supplier-list/supplier-list.component';
import { ocAppConfig } from './config/app.config';
import { OCMCheckoutConfirm } from './components/checkout/checkout-confirm/checkout-confirm.component';
import { OCMCheckout } from './components/checkout/checkout/checkout.component';
import { OCMProfile } from './components/profile/profile/profile.component';
import { OCMProfileNav } from './components/profile/profile-nav/profile-nav.component';
import { OCMAppFooter } from './components/layout/app-footer/app-footer.component';
import { OCMOrderDetails } from './components/orders/order-detail/order-detail.component';
import { OCMPaymentMethodManagement } from './components/payments/payment-method-management/payment-method-management.component';
import { OCMCheckoutPayment } from './components/checkout/checkout-payment/checkout-payment.component';
import { OCMCheckoutAddress } from './components/checkout/checkout-address/checkout-address.component';
import { OCMAddressForm } from './components/profile/address-form/address-form.component';
import { OCMGenericList } from './components/layout/generic-list/generic-list.component';
import { OCMAddressList } from './components/profile/address-list/address-list.component';
import { OCMChangePasswordForm } from './components/profile/change-password-form/change-password-form.component';
import { OCMResetPassword } from './components/authentication/reset-password/reset-password.component';
import { OCMRegister } from './components/authentication/register/register.component';
import { OCMForgotPassword } from './components/authentication/forgot-password/forgot-password.component';
import { OCMLogin } from './components/authentication/login/login.component';
import { OCMLoadingLayout } from './components/layout/loading-layout/loading-layout.component';
import { OCMOrderList } from './components/orders/order-list/order-list.component';
import { OCMOrderDateFilter } from './components/orders/order-date-filter/order-date-filter.component';
import { OCMOrderStatusFilter } from './components/orders/order-status-filter/order-status-filter.component';
import { OCMOrderStatusIcon } from './components/orders/order-status-icon/order-status-icon.component';
import { OCMModal } from './components/layout/modal/modal.component';
import { OCMProductCard } from './components/products/product-card/product-card.component';
import { OCMToggleFavorite } from './components/layout/toggle-favorite/toggle-favorite.component';
import { OCMQuantityInput } from './components/products/quantity-input/quantity-input.component';
import { OCMProductCarousel } from './components/products/product-carousel/product-carousel.component';
import { OCMProductDetails } from './components/products/product-details/product-details.component';
import { OCMImageGallery } from './components/products/image-gallery/image-gallery.component';
import { OCMSpecForm } from './components/products/spec-form/spec-form.component';
import { OCMOrderSummary } from './components/orders/order-summary/order-summary.component';
import { OCMLineitemTable } from './components/cart/lineitem-table/lineitem-table.component';
import { OCMCart } from './components/cart/cart/cart.component';
import { OCMHomePage } from './components/layout/home/home.component';
import { OCMProductSort } from './components/products/sort-products/sort-products.component';
import { OCMSupplierSort } from './components/suppliers/sort-suppliers/sort-suppliers.component';
import { OCMSupplierCard } from './components/suppliers/supplier-card/supplier-card.component';
import { OCMFacetMultiSelect } from './components/products/facet-multiselect/facet-multiselect.component';
import { OCMProductFacetList } from './components/products/product-facet-list/product-facet-list.component';
import { OCMProductList } from './components/products/product-list/product-list.component';
import { OCMSearch } from './components/layout/search/search.component';
import { OCMMiniCart } from './components/cart/mini-cart/mini-cart.component';
import { OCMAppHeader } from './components/layout/app-header/app-header.component';
import { OCMPaymentList } from './components/payments/payment-list/payment-list.component';
import { OCMAddressCard } from './components/profile/address-card/address-card.component';
import { OCMCreditCardIcon } from './components/payments/credit-card-icon/credit-card-icon.component';
import { OCMCreditCardDisplay } from './components/payments/credit-card-display/credit-card-display.component';
import { OCMCreditCardForm } from './components/payments/credit-card-form/credit-card-form.component';
import { OCMProfileForm } from './components/profile/profile-form/profile-form.component';
import { OCMCheckoutShipping } from './components/checkout/checkout-shipping/checkout-shipping.component';
import { OCMShippingSelectionForm } from './components/checkout/shipping-selection-form/shipping-selection-form.component';
import { ConfirmModal } from './components/layout/confirm-modal/confirm-modal.component.';
import { OCMPaymentCreditCard } from './components/payments/payment-credit-card/payment-credit-card.component';

const components = [
  OCMCategoryDropdown,
  OCMProductCard,
  OCMToggleFavorite,
  OCMQuantityInput,
  OCMProductCarousel,
  OCMProductDetails,
  OCMImageGallery,
  OCMSpecForm,
  OCMOrderSummary,
  OCMLineitemTable,
  OCMCart,
  OCMHomePage,
  OCMProductSort,
  OCMSupplierSort,
  OCMSupplierCard,
  OCMFacetMultiSelect,
  OCMProductFacetList,
  OCMProductList,
  OCMSearch,
  OCMMiniCart,
  OCMAppHeader,
  OCMPaymentList,
  OCMAddressCard,
  OCMCreditCardIcon,
  OCMCreditCardDisplay,
  OCMCreditCardForm,
  OCMModal,
  OCMOrderStatusIcon,
  OCMOrderStatusFilter,
  OCMOrderDateFilter,
  OCMOrderList,
  OCMLoadingLayout,
  OCMLogin,
  OCMForgotPassword,
  OCMRegister,
  OCMResetPassword,
  OCMChangePasswordForm,
  OCMAddressList,
  OCMGenericList,
  OCMAddressForm,
  OCMProfileForm,
  OCMCheckoutConfirm,
  OCMCheckoutAddress,
  OCMCheckoutPayment,
  OCMCheckout,
  OCMPaymentMethodManagement,
  OCMProfile,
  OCMProfileNav,
  OCMOrderDetails,
  OCMAppFooter,
  OCMOrderApproval,
  OCMOrderShipments,
  OCMOrderHistorical,
  OCMOrderHistory,
  OCMAppFooter,
  SpecFormCheckboxComponent,
  SpecFormInputComponent,
  SpecFormLabelComponent,
  SpecFormNumberComponent,
  SpecFormRangeComponent,
  SpecFormSelectComponent,
  SpecFormTextAreaComponent,
  OCMSupplierList,
  OCMCheckoutShipping,
  OCMShippingSelectionForm,
  ConfirmModal,
  OCMPaymentCreditCard
];

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA],
  declarations: [
    AppComponent,
    FormControlErrorDirective,
    CreditCardInputDirective,
    SpecFieldDirective,
    ProductNameWithSpecsPipe,
    OrderStatusDisplayPipe,
    PhoneFormatPipe,
    ChildCategoryPipe,
    CreditCardFormatPipe,
    PaymentMethodDisplayPipe,
    ShipperTrackingPipe,
    ShipperTrackingSupportedPipe,
    ...components,
  ],
  imports: [
    BrowserModule,
    MarketplaceModule,
    AppRoutingModule,
    HttpClientModule,
    OrderCloudModule.forRoot(OcSDKConfig),
    CookieModule.forRoot(),
    ToastrModule.forRoot(),
    NgxImageZoomModule,
    ReactiveFormsModule,
    FormsModule,
    FontAwesomeModule,
    NgbCarouselModule,
    NgbCollapseModule,
    NgbTooltipModule,
    NgbPaginationModule,
    NgbPopoverModule,
    NgbDropdownModule,
    NgbDatepickerModule,
    NgbAccordionModule,
    NgProgressModule,
    NgProgressHttpModule,
    BrowserAnimationsModule,
    NgbModule,
  ],
  providers: [
    { provide: AppConfig, useValue: ocAppConfig },
    { provide: NgbDateAdapter, useClass: NgbDateNativeAdapter },
    { provide: ErrorHandler, useClass: AppErrorHandler },
    DatePipe, // allows us to use in class as injectable (date filter component)
    CreditCardFormatPipe,
  ],
  entryComponents: components,
  bootstrap: [AppComponent],
})
export class AppModule {
  constructor(private injector: Injector, @Inject(PLATFORM_ID) private platformId: Record<string, any>) {
    this.buildWebComponent(OCMProfileNav, 'ocm-profile-nav');
    this.buildWebComponent(OCMQuantityInput, 'ocm-quantity-input');
    this.buildWebComponent(OCMProductCard, 'ocm-product-card');
    this.buildWebComponent(OCMToggleFavorite, 'ocm-toggle-favorite');
    this.buildWebComponent(OCMProductCarousel, 'ocm-product-carousel');
    this.buildWebComponent(OCMImageGallery, 'ocm-image-gallery');
    this.buildWebComponent(OCMSpecForm, 'ocm-spec-form');
    this.buildWebComponent(OCMOrderSummary, 'ocm-order-summary');
    this.buildWebComponent(OCMLineitemTable, 'ocm-lineitem-table');

    this.buildWebComponent(OCMProductDetails, 'ocm-product-details');
    this.buildWebComponent(OCMCart, 'ocm-cart');
    this.buildWebComponent(OCMHomePage, 'ocm-home-page');
    this.buildWebComponent(OCMProductSort, 'ocm-product-sort');
    this.buildWebComponent(OCMSupplierSort, 'ocm-supplier-sort');
    this.buildWebComponent(OCMSupplierCard, 'ocm-supplier-card');
    this.buildWebComponent(OCMFacetMultiSelect, 'ocm-facet-multiselect');
    this.buildWebComponent(OCMProductFacetList, 'ocm-product-facet-list');
    this.buildWebComponent(OCMProductList, 'ocm-product-list');
    this.buildWebComponent(OCMSearch, 'ocm-search');
    this.buildWebComponent(OCMMiniCart, 'ocm-mini-cart');
    this.buildWebComponent(OCMAppHeader, 'ocm-app-header');
    this.buildWebComponent(OCMCategoryDropdown, 'ocm-category-dropdown');

    this.buildWebComponent(OCMPaymentList, 'ocm-payment-list');
    this.buildWebComponent(OCMAddressCard, 'ocm-address-card');
    this.buildWebComponent(OCMCreditCardIcon, 'ocm-credit-card-icon');
    this.buildWebComponent(OCMCreditCardDisplay, 'ocm-credit-card-display');
    this.buildWebComponent(OCMCreditCardForm, 'ocm-credit-card-form');
    this.buildWebComponent(OCMModal, 'ocm-modal');
    this.buildWebComponent(OCMOrderStatusIcon, 'ocm-order-status-icon');
    this.buildWebComponent(OCMOrderStatusFilter, 'ocm-order-status-filter');
    this.buildWebComponent(OCMOrderDateFilter, 'ocm-order-date-filter');
    this.buildWebComponent(OCMOrderList, 'ocm-order-list');
    this.buildWebComponent(OCMLoadingLayout, 'ocm-loading-layout');
    this.buildWebComponent(OCMLogin, 'ocm-login');
    this.buildWebComponent(OCMForgotPassword, 'ocm-forgot-password');
    this.buildWebComponent(OCMRegister, 'ocm-register');
    this.buildWebComponent(OCMResetPassword, 'ocm-reset-password');
    this.buildWebComponent(OCMChangePasswordForm, 'ocm-change-password');
    this.buildWebComponent(OCMAddressList, 'ocm-address-list');
    this.buildWebComponent(OCMGenericList, 'ocm-generic-list');
    this.buildWebComponent(OCMAddressForm, 'ocm-address-form');
    this.buildWebComponent(OCMProfileForm, 'ocm-profile-form');

    // Alot of these checkout components will be completely re-done
    this.buildWebComponent(OCMCheckoutConfirm, 'ocm-checkout-confirm');
    this.buildWebComponent(OCMPaymentCreditCard, 'ocm-payment-credit-card');
    this.buildWebComponent(OCMCheckoutAddress, 'ocm-checkout-address');
    this.buildWebComponent(OCMCheckoutPayment, 'ocm-checkout-payment');
    this.buildWebComponent(OCMCheckout, 'ocm-checkout');
    this.buildWebComponent(OCMCheckoutShipping, 'ocm-checkout-shipping');
    this.buildWebComponent(OCMShippingSelectionForm, 'ocm-shipping-selection-form');
    this.buildWebComponent(OCMPaymentMethodManagement, 'ocm-payment-method-management');
    this.buildWebComponent(OCMProfile, 'ocm-profile');

    this.buildWebComponent(OCMOrderDetails, 'ocm-order-details');
    this.buildWebComponent(OCMAppFooter, 'ocm-app-footer');
    this.buildWebComponent(OCMOrderApproval, 'ocm-order-approval');
    this.buildWebComponent(OCMOrderShipments, 'ocm-order-shipments');
    this.buildWebComponent(OCMOrderHistorical, 'ocm-order-historical');
    this.buildWebComponent(OCMOrderHistory, 'ocm-order-history');
    this.buildWebComponent(OCMSupplierList, 'ocm-supplier-list');
    this.buildWebComponent(ConfirmModal, 'confirm-modal');
  }

  buildWebComponent(angularComponent, htmlTagName: string): void {
    const component = createCustomElement(angularComponent, {
      injector: this.injector,
      // See this issue for why this Factory, copied from Angular/elements source code is included.
      // https://github.com/angular/angular/issues/29606
      strategyFactory: new ComponentNgElementStrategyFactory(angularComponent, this.injector),
    });
    if (isPlatformBrowser(this.platformId)) {
      if (!window.customElements.get(htmlTagName)) {
        window.customElements.define(htmlTagName, component);
      }
    }
  }
}
