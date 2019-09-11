import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { SharedModule } from '@app-buyer/shared';
import { ProductsRoutingModule } from '@app-buyer/product/product-routing.module';
import { PriceFilterComponent } from '@app-buyer/product/components/price-filter/price-filter.component';
import {
  MeListProductResolver,
  MeProductResolver,
  MeListSpecsResolver,
  MeSpecsResolver,
  MeListCategoriesResolver,
  MeListRelatedProductsResolver,
} from './resolves/me.product.resolve';
import { OcmComponentsModule } from 'ocm-components';
import { OcmDefaultComponentsModule } from '@app-buyer/ocm-default-components/ocm-default-components.module';
import { ProductListWrapperComponent } from './component-wrappers/product-list-wrapper/product-list-wrapper.component';
import { ProductListComponent } from './component-wrappers/product-list/product-list.component';
import { ProductDetailWrapperComponent } from './component-wrappers/product-detail-wrapper/product-detail-wrapper.component';

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [SharedModule, ProductsRoutingModule, OcmComponentsModule, OcmDefaultComponentsModule],
  declarations: [ProductListComponent, PriceFilterComponent, ProductDetailWrapperComponent, ProductListWrapperComponent],
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
