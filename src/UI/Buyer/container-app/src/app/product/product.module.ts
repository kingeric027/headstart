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
import { OcmComponentsModule } from 'ocm-components';
import { OcmDefaultComponentsModule } from 'src/app/ocm-default-components/ocm-default-components.module';
import { ProductDetailWrapperComponent } from './components/product-detail-wrapper/product-detail-wrapper.component';
import { ProductListWrapperComponent } from './components/product-list-wrapper/product-list-wrapper.component';

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [SharedModule, ProductsRoutingModule, OcmComponentsModule, OcmDefaultComponentsModule],
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
