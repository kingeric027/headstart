import { NgModule, Injector, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { createCustomElement } from '@angular/elements';
import { OCMProductCard } from './components/product-card/product-card.component';
import { OCMToggleFavorite } from './components/toggle-favorite/toggle-favorite.component';
import { OCMQuantityInput } from './components/quantity-input/quantity-input.component';
import { OCMProductCarousel } from './components/product-carousel/product-carousel.component';
import { OCMProductDetails } from './components/product-details/product-details.component';
import { OCMImageGallery } from './components/image-gallery/image-gallery.component';
import { OCMSpecForm } from './components/spec-form/spec-form.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { NgxImageZoomModule } from 'ngx-image-zoom';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { CommonModule } from '@angular/common';
import { OCMOrderSummary } from './components/order-summary/order-summary.component';
import { OCMLineitemTable } from './components/lineitem-table/lineitem-table.component';
import { OCMCart } from './components/cart/cart.component';
import { OCMHomePage } from './components/home/home.component';
import { NgbCarouselModule, NgbCollapseModule, NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { OCMProductSort } from './components/sort-products/sort-products.component';
import { OCMCategoryTree } from './components/category-tree/category-tree.component';
import { TreeModule } from 'angular-tree-component';
import { OCMFacetMultiSelect } from './components/facet-multiselect/facet-multiselect.component';
import { OCMProductFacetList } from './components/product-facet-list/product-facet-list.component';
import { OCMProductList } from './components/product-list/product-list.component';
import { OCMSearch } from './components/search/search.component';
import { OCMMiniCart } from './components/mini-cart/mini-cart.component';
import { ProductNameWithSpecsPipe } from './pipes/product-name-with-specs/product-name-with-specs.pipe';

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  declarations: [
    OCMProductCard,
    OCMToggleFavorite,
    OCMQuantityInput,
    OCMProductCarousel,
    OCMProductDetails,
    OCMImageGallery,
    OCMSpecForm,
    OCMOrderSummary,
    OCMLineitemTable,
    OCMCart,
    OCMHomePage,
    OCMProductSort,
    OCMCategoryTree,
    OCMFacetMultiSelect,
    OCMProductFacetList,
    OCMProductList,
    OCMSearch,
    OCMMiniCart,
    ProductNameWithSpecsPipe,
  ],
  entryComponents: [
    OCMToggleFavorite,
    OCMProductCard,
    OCMQuantityInput,
    OCMProductCarousel,
    OCMProductDetails,
    OCMImageGallery,
    OCMSpecForm,
    OCMOrderSummary,
    OCMLineitemTable,
    OCMCart,
    OCMHomePage,
    OCMProductSort,
    OCMCategoryTree,
    OCMFacetMultiSelect,
    OCMProductFacetList,
    OCMProductList,
    OCMSearch,
    OCMMiniCart,
  ],
  imports: [
    CommonModule,
    NgxImageZoomModule,
    ReactiveFormsModule,
    FormsModule,
    FontAwesomeModule,
    NgbCarouselModule,
    TreeModule,
    NgbCollapseModule,
    NgbPaginationModule,
  ],
})
export class OcmDefaultComponentsModule {
  constructor(private injector: Injector) {
    this.buildWebComponent(OCMQuantityInput, 'ocm-quantity-input');
    this.buildWebComponent(OCMProductCard, 'ocm-product-card');
    this.buildWebComponent(OCMToggleFavorite, 'ocm-toggle-favorite');
    this.buildWebComponent(OCMProductCarousel, 'ocm-product-carousel');
    this.buildWebComponent(OCMImageGallery, 'ocm-image-gallery');
    this.buildWebComponent(OCMSpecForm, 'ocm-spec-form');
    this.buildWebComponent(OCMOrderSummary, 'ocm-order-summary');
    this.buildWebComponent(OCMLineitemTable, 'ocm-lineitem-table');

    this.buildWebComponent(OCMProductDetails, 'ocm-product-details');
    this.buildWebComponent(OCMCart, 'ocm-cart');
    this.buildWebComponent(OCMHomePage, 'ocm-home-page');
    this.buildWebComponent(OCMProductSort, 'ocm-product-sort');
    this.buildWebComponent(OCMCategoryTree, 'ocm-category-tree');
    this.buildWebComponent(OCMFacetMultiSelect, 'ocm-facet-multiselect');
    this.buildWebComponent(OCMProductFacetList, 'ocm-product-facet-list');
    this.buildWebComponent(OCMProductList, 'ocm-product-list');
    this.buildWebComponent(OCMSearch, 'ocm-search');
    this.buildWebComponent(OCMMiniCart, 'ocm-mini-cart');
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
