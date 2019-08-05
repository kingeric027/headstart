import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { SharedModule } from '@app-buyer/shared';
import { TreeModule } from 'angular-tree-component';
import { ProductsRoutingModule } from '@app-buyer/product/product-routing.module';
import { ProductListComponent } from '@app-buyer/product/containers/product-list/product-list.component';
import { PriceFilterComponent } from '@app-buyer/product/components/price-filter/price-filter.component';
import { CategoryNavComponent } from '@app-buyer/product/components/category-nav/category-nav.component';
import { SortFilterComponent } from '@app-buyer/product/components/sort-filter/sort-filter.component';
import { FacetFilterComponent } from '@app-buyer/product/components/facet-filter/facet-filter.component';
import { FacetListComponent } from '@app-buyer/product/components/facet-list/facet-list.component';
import { ProductDetailWrapperComponent } from './containers/product-detail-wrapper/product-detail-wrapper.component';
import { OcmModule } from '@app-buyer/ocm/ocm.module';
import {
  ListProductResolver,
  ProductResolver,
  ListSpecsResolver,
  SpecsResolver,
} from './resolves/product.resolve';

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [SharedModule, ProductsRoutingModule, TreeModule, OcmModule],
  declarations: [
    ProductListComponent,
    ProductDetailWrapperComponent,
    PriceFilterComponent,
    CategoryNavComponent,
    SortFilterComponent,
    FacetFilterComponent,
    FacetListComponent,
    ProductDetailWrapperComponent,
  ],
  providers: [
    ListProductResolver,
    ProductResolver,
    ListSpecsResolver,
    SpecsResolver,
  ],
})
export class ProductsModule {}
