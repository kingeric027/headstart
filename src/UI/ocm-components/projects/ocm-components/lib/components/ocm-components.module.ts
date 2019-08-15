import { NgModule, Injector, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { createCustomElement } from '@angular/elements';
import { OCMProductCard } from './product-card/product-card.component';
import { OCMToggleFavorite } from './toggle-favorite/toggle-favorite.component';
import { OCMQuantityInput } from './quantity-input/quantity-input.component';
import { OCMProductCarousel } from './product-carousel/product-carousel.component';
import { OCMProductDetails } from './product-details/product-details.component';
import { OCMImageGallery } from './image-gallery/image-gallery.component';
import { OCMSpecForm } from './spec-form/spec-form.component';
import { ReactiveFormsModule } from '@angular/forms';
import { NgxImageZoomModule } from 'ngx-image-zoom';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { CommonModule } from '@angular/common';



@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  declarations: [
    OCMProductCard,
    OCMToggleFavorite,
    OCMQuantityInput,
    OCMProductCarousel,
    OCMProductDetails,
    OCMImageGallery,
    OCMSpecForm
  ],
  entryComponents: [
    OCMToggleFavorite,
    OCMProductCard,
    OCMQuantityInput,
    OCMProductCarousel,
    OCMProductDetails,
    OCMImageGallery,
    OCMSpecForm
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
