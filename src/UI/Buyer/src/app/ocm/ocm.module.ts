import { NgModule } from '@angular/core';
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

@NgModule({
  declarations: [
    OCMQuantityInput,
    OCMToggleFavorite,
    OCMProductCard,
    OCMProductCarousel,
    OCMProductDetails,
    OCMSpecForm,
    OCMImageGallery,
  ],
  exports: [
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
export class OcmModule {}
