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
import { OCMProductCard } from './components/product-card/product-card.component';
import { OCMToggleFavorite } from './components/toggle-favorite/toggle-favorite.component';
import { OCMQuantityInput } from './components/quantity-input/quantity-input.component';
import { OCMProductCarousel } from './components/product-carousel/product-carousel.component';
import { OCMProductDetails } from './components/product-details/product-details.component';
import { OCMImageGallery } from './components/image-gallery/image-gallery.component';
import { OCMSpecForm } from './components/spec-form/spec-form.component';
import { OCMOrderSummary } from './components/order-summary/order-summary.component';
import { OCMCart } from './components/cart/cart.component';
import { OCMLineitemTable } from './components/lineitem-table/lineitem-table.component';
import { OCMHomePage } from './components/home/home.component';
import { OCMProductSort } from './components/sort-products/sort-products.component';
import { OCMSupplierSort } from './components/sort-suppliers/sort-suppliers.component';
import { OCMSupplierCard } from './components/supplier-card/supplier-card.component';
import { OCMFacetMultiSelect } from './components/facet-multiselect/facet-multiselect.component';
import { OCMProductFacetList } from './components/product-facet-list/product-facet-list.component';
import { OCMProductList } from './components/product-list/product-list.component';
import { OCMSearch } from './components/search/search.component';
import { OCMMiniCart } from './components/mini-cart/mini-cart.component';
import { OCMPaymentList } from './components/payment-list/payment-list.component';
import { OCMAppHeader } from './components/app-header/app-header.component';
import { OCMAddressCard } from './components/address-card/address-card.component';
import { OCMCreditCardIcon } from './components/credit-card-icon/credit-card-icon.component';
import { OCMCreditCardDisplay } from './components/credit-card-display/credit-card-display.component';
import { OCMModal } from './components/modal/modal.component';
import { OCMOrderStatusIcon } from './components/order-status-icon/order-status-icon.component';
import { OCMCreditCardForm } from './components/credit-card-form/credit-card-form.component';
import { OCMOrderStatusFilter } from './components/order-status-filter/order-status-filter.component';
import { OCMOrderDateFilter } from './components/order-date-filter/order-date-filter.component';
import { OCMOrderList } from './components/order-list/order-list.component';
import { OCMLogin } from './components/login/login.component';
import { OCMForgotPassword } from './components/forgot-password/forgot-password.component';
import { OCMRegister } from './components/register/register.component';
import { OCMResetPassword } from './components/reset-password/reset-password.component';
import { OCMChangePasswordForm } from './components/change-password-form/change-password-form.component';
import { OCMAddressList } from './components/address-list/address-list.component';
import { OMCAddressForm } from './components/address-form/address-form.component';
import { OCMGenericList } from './components/generic-list/generic-list.component';
import { OCMCheckoutConfirm } from './components/checkout-confirm/checkout-confirm.component';
import { OCMPaymentPurchaseOrder } from './components/payment-purchase-order/payment-purchase-order.component';
import { OCMPaymentSpendingAccount } from './components/payment-spending-account/payment-spending-account.component';
import { OCMCheckoutAddress } from './components/checkout-address/checkout-address.component';
import { OCMCheckoutPayment } from './components/checkout-payment/checkout-payment.component';
import { OCMCheckout } from './components/checkout/checkout.component';
import { OCMPaymentMethodManagement } from './components/payment-method-management/payment-method-management.component';
import { OCMProfile } from './components/profile/profile.component';
import { OCMProfileNav } from './components/profile-nav/profile-nav.component';
import { OCMOrderDetails } from './components/order-detail/order-detail.component';
import { OCMAppFooter } from './components/app-footer/app-footer.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { TreeModule } from 'angular-tree-component';
import { NgxImageZoomModule } from 'ngx-image-zoom';
import {
  NgbCarouselModule,
  NgbCollapseModule,
  NgbPaginationModule,
  NgbPopoverModule,
  NgbDropdownModule,
  NgbDatepickerModule,
  NgbAccordionModule,
  NgbDateAdapter,
} from '@ng-bootstrap/ng-bootstrap';
import { FormControlErrorDirective } from './directives/form-control-errors.directive';
import { CreditCardInputDirective } from './directives/credit-card-input.directive';
import { ProductNameWithSpecsPipe } from './pipes/product-name-with-specs.pipe';
import { OrderStatusDisplayPipe } from './pipes/order-status-display.pipe';
import { CreditCardFormatPipe } from './pipes/credit-card-format.pipe';
import { PaymentMethodDisplayPipe } from './pipes/payment-method-display.pipe';
import { HttpClientModule } from '@angular/common/http';
import { OcSDKConfig } from './config/ordercloud-sdk.config';
import { ComponentNgElementStrategyFactory } from 'src/lib/component-factory-strategy';
import { NgbDateNativeAdapter } from './config/date-picker.config';
import { AppErrorHandler } from './config/error-handling.config';
import { NgProgressModule } from '@ngx-progressbar/core';
import { NgProgressHttpModule } from '@ngx-progressbar/http';
import { OCMReorder } from './components/re-order/re-order.component';
import { OCMOrderApproval } from './components/order-approval/order-approval.component';
import { OCMOrderShipments } from './components/order-shipments/order-shipments.component';
import { ShipperTrackingPipe, ShipperTrackingSupportedPipe } from './pipes/shipperTracking.pipe';
import { OCMOrderHistorical } from './components/order-historical/order-historical.component';
import { OCMOrderHistory } from './components/order-history/order-history.component';

import { SpecFieldDirective } from './components/spec-form/spec-field.directive';
import { SpecFormCheckboxComponent } from './components/spec-form/spec-form-checkbox/spec-form-checkbox.component';
import { SpecFormInputComponent } from './components/spec-form/spec-form-input/spec-form-input.component';
import { SpecFormLabelComponent } from './components/spec-form/spec-form-label/spec-form-label.component';
import { SpecFormNumberComponent } from './components/spec-form/spec-form-number/spec-form-number.component';
import { SpecFormRangeComponent } from './components/spec-form/spec-form-range/spec-form-range.component';
import { SpecFormSelectComponent } from './components/spec-form/spec-form-select/spec-form-select.component';
import { SpecFormTextAreaComponent } from './components/spec-form/spec-form-textarea/spec-form-textarea.component';
import { OCMSupplierList } from './components/supplier-list/supplier-list.component';
import { ocAppConfig } from './config/app.config';

const components = [
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
  OCMLogin,
  OCMForgotPassword,
  OCMRegister,
  OCMResetPassword,
  OCMChangePasswordForm,
  OCMAddressList,
  OCMGenericList,
  OMCAddressForm,
  OCMCheckoutConfirm,
  OCMPaymentPurchaseOrder,
  OCMPaymentSpendingAccount,
  OCMCheckoutAddress,
  OCMCheckoutPayment,
  OCMCheckout,
  OCMPaymentMethodManagement,
  OCMProfile,
  OCMProfileNav,
  OCMOrderDetails,
  OCMAppFooter,
  OCMReorder,
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
    TreeModule,
    NgbCarouselModule,
    NgbCollapseModule,
    NgbPaginationModule,
    NgbPopoverModule,
    NgbDropdownModule,
    NgbDatepickerModule,
    NgbAccordionModule,
    NgProgressModule,
    NgProgressHttpModule,
    BrowserAnimationsModule,
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
  constructor(private injector: Injector, @Inject(PLATFORM_ID) private platformId: Object) {
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
    this.buildWebComponent(OCMLogin, 'ocm-login');
    this.buildWebComponent(OCMForgotPassword, 'ocm-forgot-password');
    this.buildWebComponent(OCMRegister, 'ocm-register');
    this.buildWebComponent(OCMResetPassword, 'ocm-reset-password');
    this.buildWebComponent(OCMChangePasswordForm, 'ocm-change-password');
    this.buildWebComponent(OCMAddressList, 'ocm-address-list');
    this.buildWebComponent(OCMGenericList, 'ocm-generic-list');
    this.buildWebComponent(OMCAddressForm, 'ocm-address-form');

    // Alot of these checkout components will be completely re-done
    this.buildWebComponent(OCMCheckoutConfirm, 'ocm-checkout-confirm');
    this.buildWebComponent(OCMPaymentSpendingAccount, 'ocm-payment-spending-account');
    this.buildWebComponent(OCMPaymentPurchaseOrder, 'ocm-payment-purchase-order');
    this.buildWebComponent(OCMCheckoutAddress, 'ocm-checkout-address');
    this.buildWebComponent(OCMCheckoutPayment, 'ocm-checkout-payment');
    this.buildWebComponent(OCMCheckout, 'ocm-checkout');
    this.buildWebComponent(OCMPaymentMethodManagement, 'ocm-payment-method-management');
    this.buildWebComponent(OCMProfile, 'ocm-profile');

    this.buildWebComponent(OCMOrderDetails, 'ocm-order-details');
    this.buildWebComponent(OCMAppFooter, 'ocm-app-footer');
    this.buildWebComponent(OCMReorder, 'ocm-reorder');
    this.buildWebComponent(OCMOrderApproval, 'ocm-order-approval');
    this.buildWebComponent(OCMOrderShipments, 'ocm-order-shipments');
    this.buildWebComponent(OCMOrderHistorical, 'ocm-order-historical');
    this.buildWebComponent(OCMOrderHistory, 'ocm-order-history');
    this.buildWebComponent(OCMSupplierList, 'ocm-supplier-list');
  }

  buildWebComponent(angularComponent, htmlTagName: string) {
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
