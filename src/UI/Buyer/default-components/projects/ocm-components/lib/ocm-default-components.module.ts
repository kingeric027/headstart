import { NgModule, Injector, PLATFORM_ID, CUSTOM_ELEMENTS_SCHEMA, Inject } from '@angular/core';
import { createCustomElement } from '@angular/elements';
import { OCMProductCard } from './components/product-card/product-card.component';
import { OCMToggleFavorite } from './components/toggle-favorite/toggle-favorite.component';
import { OCMQuantityInput } from './components/quantity-input/quantity-input.component';
import { OCMProductCarousel } from './components/product-carousel/product-carousel.component';
import { OCMProductDetails } from './components/product-details/product-details.component';
import { OCMImageGallery } from './components/image-gallery/image-gallery.component';
import { OCMSpecForm } from './components/spec-form/spec-form.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { NgxImageZoomModule } from 'ngx-image-zoom';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { CommonModule, isPlatformBrowser, DatePipe } from '@angular/common';
import { OCMOrderSummary } from './components/order-summary/order-summary.component';
import { OCMLineitemTable } from './components/lineitem-table/lineitem-table.component';
import { OCMCart } from './components/cart/cart.component';
import { OCMHomePage } from './components/home/home.component';
import {
  NgbCarouselModule,
  NgbCollapseModule,
  NgbPaginationModule,
  NgbPopoverModule,
  NgbDropdownModule,
  NgbDatepickerModule,
} from '@ng-bootstrap/ng-bootstrap';
import { OCMProductSort } from './components/sort-products/sort-products.component';
import { OCMFacetMultiSelect } from './components/facet-multiselect/facet-multiselect.component';
import { OCMProductFacetList } from './components/product-facet-list/product-facet-list.component';
import { OCMProductList } from './components/product-list/product-list.component';
import { OCMSearch } from './components/search/search.component';
import { OCMMiniCart } from './components/mini-cart/mini-cart.component';
import { ProductNameWithSpecsPipe } from './pipes/product-name-with-specs/product-name-with-specs.pipe';
import { OCMAppHeader } from './components/app-header/app-header.component';
import { OCMPaymentList } from './components/payment-list/payment-list.component';
import { OCMAddressCard } from './components/address-card/address-card.component';
import { OCMCreditCardIcon } from './components/credit-card-icon/credit-card-icon.component';
import { OCMCreditCardDisplay } from './components/credit-card-display/credit-card-display.component';
import { OCMCreditCardForm } from './components/credit-card-form/credit-card-form.component';
import { OCMModal } from './components/modal/modal.component';
import { OCMOrderStatusIcon } from './components/order-status-icon/order-status-icon.component';
import { OCMOrderStatusFilter } from './components/order-status-filter/order-status-filter.component';
import { OrderStatusDisplayPipe } from './pipes/order-status-display/order-status-display.pipe';
import { OCMOrderDateFilter } from './components/order-date-filter/order-date-filter.component';
import { OCMOrderList } from './components/order-list/order-list.component';
import { OCMLogin } from './components/login/login.component';
import { OCMForgotPassword } from './components/forgot-password/forgot-password.component';
import { OCMRegister } from './components/register/register.component';
import { OCMResetPassword } from './components/reset-password/reset-password.component';
import { OCMMeUpdateComponent } from './components/me-update/me-update.component';
import { OCMChangePasswordForm } from './components/change-password-form/change-password-form.component';
import { FormControlErrorDirective } from './directives/form-control-errors.directive';
import { OCMAddressList } from './components/address-list/address-list.component';
import { TreeModule } from 'angular-tree-component';
import { OCMCategoryTree } from './components/category-tree/category-tree.component';
import { OCMGenericList } from './components/generic-list/generic-list.component';
import { OMCAddressForm } from './components/address-form/address-form.component';

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  declarations: [
    FormControlErrorDirective,

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
    OCMCategoryTree,
    OCMFacetMultiSelect,
    OCMProductFacetList,
    OCMProductList,
    OCMSearch,
    OCMMiniCart,
    ProductNameWithSpecsPipe,
    OrderStatusDisplayPipe,
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
    OCMMeUpdateComponent,
    OCMChangePasswordForm,
    OCMAddressList,
    OCMGenericList,
    OMCAddressForm
  ],
  entryComponents: [
    OCMToggleFavorite,
    OCMProductCard,
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
    OCMCategoryTree,
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
    OCMMeUpdateComponent,
    OCMChangePasswordForm,
    OCMAddressList,
    OCMGenericList,
    OMCAddressForm
  ],
  imports: [
    CommonModule,
    NgxImageZoomModule,
    ReactiveFormsModule,
    FormsModule,
    FontAwesomeModule,
    NgbCarouselModule,
    TreeModule,
    NgbCollapseModule,
    NgbPaginationModule,
    NgbPopoverModule,
    NgbDropdownModule,
    NgbDatepickerModule,
  ],
  providers: [
    DatePipe, // allows us to use in class as injectable (date filter component)
  ],
})
export class OcmDefaultComponentsModule {
  constructor(private injector: Injector, @Inject(PLATFORM_ID) private platformId: Object) {
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
    this.buildWebComponent(OCMCategoryTree, 'ocm-category-tree');
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
    this.buildWebComponent(OCMMeUpdateComponent, 'ocm-profile-me-update');
    this.buildWebComponent(OCMChangePasswordForm, 'ocm-change-password');
    this.buildWebComponent(OCMAddressList, 'ocm-address-list');
    this.buildWebComponent(OCMGenericList, 'ocm-generic-list');
    this.buildWebComponent(OMCAddressForm, 'ocm-address-form');
  }

  buildWebComponent(angularComponent, htmlTagName: string) {
    const component = createCustomElement(angularComponent, {
      injector: this.injector,
    });
    if (isPlatformBrowser(this.platformId)) {
      if (!window.customElements.get(htmlTagName)) {
        window.customElements.define(htmlTagName, component);
      }
    }
  }
}
