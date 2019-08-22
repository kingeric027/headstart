import { NgModule, Injector, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { createCustomElement } from '@angular/elements';
import { OCMProductCard } from './exposed/product-card/product-card.component';
import { OCMToggleFavorite } from './exposed/toggle-favorite/toggle-favorite.component';
import { OCMQuantityInput } from './exposed/quantity-input/quantity-input.component';
import { OCMProductCarousel } from './exposed/product-carousel/product-carousel.component';
import { OCMProductDetails } from './exposed/product-details/product-details.component';
import { OCMImageGallery } from './internal/image-gallery/image-gallery.component';
import { OCMSpecForm } from './internal/spec-form/spec-form.component';
import { ReactiveFormsModule } from '@angular/forms';
import { NgxImageZoomModule } from 'ngx-image-zoom';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { CommonModule } from '@angular/common';
import { OCMOrderSummary } from './exposed/order-summary/order-summary.component';
import { OCMLineitemTable } from './exposed/lineitem-table/lineitem-table.component';

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  declarations: [
    OCMProductCard,
    OCMToggleFavorite,
    OCMQuantityInput,
    OCMProductCarousel,
    OCMProductDetails,
    OCMImageGallery,
    OCMSpecForm,
    OCMOrderSummary,
    OCMLineitemTable
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
    OCMLineitemTable
  ],
  imports: [
    CommonModule,
    NgxImageZoomModule,
    ReactiveFormsModule,
    FontAwesomeModule,
  ],
})
export class OcmComponentsModule {
  constructor(private injector: Injector) {
    this.buildWebComponent(OCMQuantityInput, 'ocm-quantity-input');
    this.buildWebComponent(OCMProductCard, 'ocm-product-card');
    this.buildWebComponent(OCMToggleFavorite, 'ocm-toggle-favorite');
    this.buildWebComponent(OCMProductDetails, 'ocm-product-details');
    this.buildWebComponent(OCMProductCarousel, 'ocm-product-carousel');
    this.buildWebComponent(OCMImageGallery, 'ocm-image-gallery');
    this.buildWebComponent(OCMSpecForm, 'ocm-spec-form');
    this.buildWebComponent(OCMOrderSummary, 'ocm-order-summary');
    this.buildWebComponent(OCMLineitemTable, 'ocm-lineitem-table');
  }

  buildWebComponent(angularComponent, htmlTagName: string) {
    const component = createCustomElement(angularComponent, {
      injector: this.injector,
    });
    if (!customElements.get(htmlTagName)) {
      customElements.define(htmlTagName, component);
    }
 }
}
