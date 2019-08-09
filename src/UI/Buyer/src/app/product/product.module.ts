import { NgModule } from '@angular/core';
import { SharedModule } from '@app-buyer/shared';
import { TreeModule } from 'angular-tree-component';
import { ProductsRoutingModule } from '@app-buyer/product/product-routing.module';
import { ProductListComponent } from '@app-buyer/product/containers/product-list/product-list.component';
import { ProductDetailsComponent } from '@app-buyer/product/containers/product-details/product-details.component';
import { AdditionalImageGalleryComponent } from '@app-buyer/product/components/additional-image-gallery/additional-image-gallery.component';
import { PriceFilterComponent } from '@app-buyer/product/components/price-filter/price-filter.component';
import { CategoryNavComponent } from '@app-buyer/product/components/category-nav/category-nav.component';
import { SortFilterComponent } from '@app-buyer/product/components/sort-filter/sort-filter.component';
import { FacetFilterComponent } from '@app-buyer/product/components/facet-filter/facet-filter.component';
import { FacetListComponent } from '@app-buyer/product/components/facet-list/facet-list.component';
import { NgxImageZoomModule } from 'ngx-image-zoom';
import { SpecFormComponent } from './components/spec-form/spec-form.component';
import {
  MeListProductResolver,
  MeProductResolver,
  MeListSpecsResolver,
  MeSpecsResolver,
  MeListCategoriesResolver,
  MeListRelatedProductsResolver,
} from './resolves/me.product.resolve';
import { SpecFieldDirective } from './components/spec-form/spec-field.directive';
import { SpecFormTextAreaComponent } from './components/spec-form/spec-form-textarea/spec-form-textarea.component';
import { SpecFormButtonComponent } from './components/spec-form/spec-form-button/spec-form-button.component';
import { SpecFormInputComponent } from './components/spec-form/spec-form-input/spec-form-input.component';
import { SpecFormNumberComponent } from './components/spec-form/spec-form-number/spec-form-number.component';
import { SpecFormSelectComponent } from './components/spec-form/spec-form-select/spec-form-select.component';
import { SpecFormAddToCartComponent } from './components/spec-form/spec-form-add-to-cart/spec-form-add-to-cart.component';

@NgModule({
  imports: [
    SharedModule,
    ProductsRoutingModule,
    TreeModule,
    NgxImageZoomModule,
  ],
  declarations: [
    ProductListComponent,
    ProductDetailsComponent,
    AdditionalImageGalleryComponent,
    PriceFilterComponent,
    CategoryNavComponent,
    SortFilterComponent,
    FacetFilterComponent,
    FacetListComponent,
    SpecFormComponent,
    SpecFieldDirective,
    SpecFormTextAreaComponent,
    SpecFormButtonComponent,
    SpecFormInputComponent,
    SpecFormNumberComponent,
    SpecFormSelectComponent,
    SpecFormAddToCartComponent,
  ],
  providers: [
    MeListProductResolver,
    MeProductResolver,
    MeListSpecsResolver,
    MeSpecsResolver,
    MeListCategoriesResolver,
    MeListRelatedProductsResolver,
  ],
  entryComponents: [
    SpecFormTextAreaComponent,
    SpecFormButtonComponent,
    SpecFormInputComponent,
    SpecFormNumberComponent,
    SpecFormSelectComponent,
    SpecFormAddToCartComponent,
  ],
})
export class ProductsModule {}
