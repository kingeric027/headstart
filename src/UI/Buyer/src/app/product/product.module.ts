import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { SharedModule } from '@app-buyer/shared';
import { ProductsRoutingModule } from '@app-buyer/product/product-routing.module';
import { ProductListComponent } from '@app-buyer/product/containers/product-list/product-list.component';
import { PriceFilterComponent } from '@app-buyer/product/components/price-filter/price-filter.component';
import { FacetFilterComponent } from '@app-buyer/product/components/facet-filter/facet-filter.component';
import { FacetListComponent } from '@app-buyer/product/components/facet-list/facet-list.component';
import { ProductDetailWrapperComponent } from './containers/product-detail-wrapper/product-detail-wrapper.component';
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

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [SharedModule, ProductsRoutingModule, OcmComponentsModule, OcmDefaultComponentsModule],
  declarations: [
    ProductListComponent,
    ProductDetailWrapperComponent,
    PriceFilterComponent,
    FacetFilterComponent,
    FacetListComponent,
    ProductDetailWrapperComponent,
  ],
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
