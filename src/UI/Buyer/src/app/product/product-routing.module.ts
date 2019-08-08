// core services
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

// product components
import { ProductListComponent } from '@app-buyer/product/containers/product-list/product-list.component';
import { ProductDetailsComponent } from '@app-buyer/product/containers/product-details/product-details.component';
import {
  MeProductResolver,
  MeListSpecsResolver,
  MeListProductResolver,
  MeListCategoriesResolver,
  MeListRelatedProductsResolver,
} from './resolves/me.product.resolve';

const routes: Routes = [
  {
    path: '',
    component: ProductListComponent,
    resolve: {
      products: MeListProductResolver,
      categories: MeListCategoriesResolver,
    },
    runGuardsAndResolvers: 'always',
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
        component: ProductDetailsComponent,
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
