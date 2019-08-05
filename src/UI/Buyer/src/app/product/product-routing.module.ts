// core services
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

// product components
import { ProductListComponent } from '@app-buyer/product/containers/product-list/product-list.component';
import { ProductDetailWrapperComponent } from './containers/product-detail-wrapper/product-detail-wrapper.component';
import {
  ListProductResolver,
  ListSpecsResolver,
  ProductResolver,
} from './resolves/product.resolve';

const routes: Routes = [
  {
    path: '',
    component: ProductListComponent,
    resolve: {
      products: ListProductResolver,
    },
  },
  {
    path: ':productID',
    resolve: {
      product: ProductResolver,
      specList: ListSpecsResolver,
    },
    children: [
      {
        path: '',
        component: ProductDetailWrapperComponent,
        resolve: {
          specs: ListSpecsResolver,
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
