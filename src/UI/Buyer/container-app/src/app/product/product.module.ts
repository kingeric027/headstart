import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { SharedModule } from 'src/app/shared';
import { ProductsRoutingModule } from 'src/app/product/product-routing.module';
import {
  MeListProductResolver,
  MeProductResolver,
  MeListSpecsResolver,
  MeSpecsResolver,
  MeListCategoriesResolver,
  MeListRelatedProductsResolver,
} from './resolves/me.product.resolve';
import { ProductDetailWrapperComponent } from './components/product-detail-wrapper/product-detail-wrapper.component';
import { ProductListWrapperComponent } from './components/product-list-wrapper/product-list-wrapper.component';
import { OcmDefaultComponentsModule } from 'ocm-components';

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [SharedModule, ProductsRoutingModule, OcmDefaultComponentsModule],
  declarations: [ProductDetailWrapperComponent, ProductListWrapperComponent],
  providers: [
    MeListProductResolver,
    MeProductResolver,
    MeListSpecsResolver,
    MeSpecsResolver,
    MeListCategoriesResolver,
    MeListRelatedProductsResolver,
  ],
})
export class ProductsModule {}
