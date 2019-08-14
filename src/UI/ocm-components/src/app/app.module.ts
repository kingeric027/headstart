import { BrowserModule } from '@angular/platform-browser';
import { NgModule, Injector, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { AppComponent } from './app.component';
import { OCMToggleFavorite } from './components/toggle-favorite/toggle-favorite.component';
import { createCustomElement } from '@angular/elements';
import { OCMQuantityInput } from './components/quantity-input/quantity-input.component';
import { OCMProductCard } from './components/product-card/product-card.component';
import { OCMProductDetails } from './components/product-details/product-details.component';
import { OCMProductCarousel } from './components/product-carousel/product-carousel.component';
import { ReactiveFormsModule } from '@angular/forms';
import { OCMImageGallery } from './components/image-gallery/image-gallery.component';
import { OCMSpecForm } from './components/spec-form/spec-form.component';
import { NgxImageZoomModule } from 'ngx-image-zoom';

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  declarations: [
    AppComponent,
    OCMProductCard,
    OCMToggleFavorite,
    OCMQuantityInput,
    OCMProductCarousel,
    OCMProductDetails,
    OCMImageGallery,
    OCMSpecForm
  ],
  imports: [
    NgxImageZoomModule,
    ReactiveFormsModule,
    FontAwesomeModule,
    BrowserModule
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
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule {
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
