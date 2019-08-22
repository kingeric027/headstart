// angular
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';

import { SharedRoutingModule } from '@app-buyer/shared/shared-routing.module';

// 3rd party UI
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { TextMaskModule } from 'angular2-text-mask';

// ng-bootstrap modules
import {
  NgbCollapseModule,
  NgbDatepickerModule,
  NgbTabsetModule,
  NgbPaginationModule,
  NgbPopoverModule,
  NgbAccordionModule,
  NgbModalModule,
  NgbCarouselModule,
  NgbDropdownModule,
} from '@ng-bootstrap/ng-bootstrap';

// pipes
import { OrderStatusDisplayPipe } from '@app-buyer/shared/pipes/order-status-display/order-status-display.pipe';
import { PaymentMethodDisplayPipe } from '@app-buyer/shared/pipes/payment-method-display/payment-method-display.pipe';

// directives
import { FallbackImageDirective } from './directives/fallback-image/fallback-image.directive';

// components
import { AddressFormComponent } from '@app-buyer/shared/components/address-form/address-form.component';
import { CreditCardFormComponent } from '@app-buyer/shared/components/credit-card-form/credit-card-form.component';
import { CreditCardIconComponent } from '@app-buyer/shared/components/credit-card-icon/credit-card-icon.component';
import { SearchComponent } from '@app-buyer/shared/components/search/search.component';
import { AddressDisplayComponent } from '@app-buyer/shared/components/address-display/address-display.component';
import { CreditCardDisplayComponent } from '@app-buyer/shared/components/credit-card-display/credit-card-display.component';

// containers
import { ShipperTrackingPipe, ShipperTrackingSupportedPipe } from '@app-buyer/shared/pipes/shipperTracking/shipperTracking.pipe';
import { MapToIterablePipe } from '@app-buyer/shared/pipes/map-to-iterable/map-to-iterable.pipe';
import { GenericBrowseComponent } from '@app-buyer/shared/components/generic-browse/generic-browse.component';
import { ModalComponent, ResetDirective } from '@app-buyer/shared/components/modal/modal.component';
import { OrderPaymentListComponent } from '@app-buyer/shared/components/payment-list/order-payment-list.component';
import { NoResultsComponent } from './components/no-results/no-results.component';
import { MiniCartComponent } from './components/mini-cart/mini-cart.component';

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [
    SharedRoutingModule,
    // angular
    CommonModule,
    HttpClientModule,
    ReactiveFormsModule,

    // 3rd party UI
    FontAwesomeModule,
    TextMaskModule,

    /**
     * ng-bootstrap modules
     * only those that are used by app
     * should be imported to reduce bundle size
     */
    NgbDatepickerModule.forRoot(),
    NgbCollapseModule.forRoot(),
    NgbModalModule.forRoot(),
    NgbTabsetModule.forRoot(),
    NgbPaginationModule.forRoot(),
    NgbPopoverModule.forRoot(),
    NgbAccordionModule.forRoot(),
    NgbCarouselModule.forRoot(),
    NgbDropdownModule.forRoot(),
  ],
  exports: [
    // angular
    CommonModule,
    HttpClientModule,
    ReactiveFormsModule,

    // 3rd party UI
    FontAwesomeModule,
    TextMaskModule,

    NgbDatepickerModule,
    NgbCollapseModule,
    NgbModalModule,
    NgbTabsetModule,
    NgbPaginationModule,
    NgbPopoverModule,
    NgbAccordionModule,
    NgbCarouselModule,
    NgbDropdownModule,

    NoResultsComponent,
    SearchComponent,
    OrderStatusDisplayPipe,
    PaymentMethodDisplayPipe,
    MapToIterablePipe,
    CreditCardIconComponent,
    AddressDisplayComponent,
    CreditCardDisplayComponent,
    CreditCardFormComponent,
    FallbackImageDirective,
    ResetDirective,
    ShipperTrackingPipe,
    ShipperTrackingSupportedPipe,
    AddressFormComponent,

    GenericBrowseComponent,
    ModalComponent,
    OrderPaymentListComponent,
    MiniCartComponent,
  ],
  declarations: [
    OrderStatusDisplayPipe,
    PaymentMethodDisplayPipe,
    MapToIterablePipe,
    AddressDisplayComponent,
    CreditCardDisplayComponent,
    CreditCardFormComponent,
    FallbackImageDirective,
    ResetDirective,
    SearchComponent,
    CreditCardIconComponent,
    ShipperTrackingPipe,
    ShipperTrackingSupportedPipe,
    AddressFormComponent,
    MiniCartComponent,
    GenericBrowseComponent,
    ModalComponent,
    OrderPaymentListComponent,
    NoResultsComponent,
  ],
})
export class SharedModule {}
