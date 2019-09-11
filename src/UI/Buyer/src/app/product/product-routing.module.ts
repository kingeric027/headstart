// core services
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

// product components
import {
  MeListProductResolver,
  MeListSpecsResolver,
  MeProductResolver,
  MeListCategoriesResolver,
  MeListRelatedProductsResolver,
} from './resolves/me.product.resolve';
import { ProductListWrapperComponent } from './components/product-list-wrapper/product-list-wrapper.component';
import { ProductDetailWrapperComponent } from './components/product-detail-wrapper/product-detail-wrapper.component';

const routes: Routes = [
  {
    path: '',
    component: ProductListWrapperComponent,
    resolve: {
      products: MeListProductResolver,
      categories: MeListCategoriesResolver,
    },
  },
  {
    path: ':productID',
    resolve: {
      product: MeProductResolver,
      specList: MeListSpecsResolver,
    },
    children: [
      {
        path: '',
        component: ProductDetailWrapperComponent,
        resolve: {
          specs: MeListSpecsResolver,
          relatedProducts: MeListRelatedProductsResolver,
        },
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ProductsRoutingModule {}
