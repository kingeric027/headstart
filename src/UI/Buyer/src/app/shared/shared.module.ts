// angular
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';

import { SharedRoutingModule } from 'src/app/shared/shared-routing.module';

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
import { PaymentMethodDisplayPipe } from 'src/app/shared/pipes/payment-method-display/payment-method-display.pipe';

// directives
import { FallbackImageDirective } from './directives/fallback-image/fallback-image.directive';

// components
import { AddressFormComponent } from 'src/app/shared/components/address-form/address-form.component';

// containers
import { ShipperTrackingPipe, ShipperTrackingSupportedPipe } from 'src/app/shared/pipes/shipperTracking/shipperTracking.pipe';
import { GenericBrowseComponent } from 'src/app/shared/components/generic-browse/generic-browse.component';
import { PhoneFormatPipe } from './pipes/phone-format/phone-format.pipe';
import { PhoneInputDirective } from './directives/phone-input/phone-input.directive';

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
    PaymentMethodDisplayPipe,
    FallbackImageDirective,
    ShipperTrackingPipe,
    ShipperTrackingSupportedPipe,
    AddressFormComponent,
    PhoneFormatPipe,
    PhoneInputDirective,
    GenericBrowseComponent,
  ],
  declarations: [
    PaymentMethodDisplayPipe,
    FallbackImageDirective,
    ShipperTrackingPipe,
    ShipperTrackingSupportedPipe,
    PhoneFormatPipe,
    PhoneInputDirective,
    AddressFormComponent,
    GenericBrowseComponent,
  ],
})
export class SharedModule {}
