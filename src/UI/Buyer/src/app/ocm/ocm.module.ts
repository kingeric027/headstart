import { NgModule, Injector, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OCMSpecForm } from './spec-form/spec-form.component';
import { ReactiveFormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { RouterModule } from '@angular/router';
import { NgxImageZoomModule } from 'ngx-image-zoom';
import { OCMImageGallery } from './image-gallery/image-gallery.component';
import { OCMProductCard } from './product-card/product-card.component';
import { OCMProductCarousel } from './product-carousel/product-carousel.component';
import { OCMProductDetails } from './product-details/product-details.component';
import { OCMQuantityInput } from './quantity-input/quantity-input.component';
import { OCMToggleFavorite } from './toggle-favorite/toggle-favorite.component';
import { createCustomElement } from '@angular/elements';

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  declarations: [
    OCMQuantityInput,
    OCMToggleFavorite,
    OCMProductCard,
    OCMProductCarousel,
    OCMProductDetails,
    OCMSpecForm,
    OCMImageGallery,
  ],
  entryComponents: [
    OCMQuantityInput,
    OCMToggleFavorite,
    OCMProductCard,
    OCMProductCarousel,
    OCMProductDetails,
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FontAwesomeModule,
    RouterModule,
    NgxImageZoomModule.forRoot(),
  ],
})
export class OcmModule {
  constructor(private injector: Injector) {
    this.buildWebComponent(OCMQuantityInput, 'ocm-quantity-input');
    this.buildWebComponent(OCMProductCard, 'ocm-product-card');
    this.buildWebComponent(OCMToggleFavorite, 'ocm-toggle-favorite');
    this.buildWebComponent(OCMProductDetails, 'ocm-product-detials');
    this.buildWebComponent(OCMProductCarousel, 'ocm-product-carousel');
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
