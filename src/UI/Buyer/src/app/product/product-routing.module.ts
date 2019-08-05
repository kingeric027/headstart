// core services
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

// product components
import { ProductListComponent } from '@app-buyer/product/containers/product-list/product-list.component';
import { ProductDetailWrapperComponent } from './containers/product-detail-wrapper/product-detail-wrapper.component';
import {
  MeListProductResolver,
  MeListSpecsResolver,
  MeProductResolver,
  MeListCategoriesResolver,
} from './resolves/me.resolve';

const routes: Routes = [
  {
    path: '',
    component: ProductListComponent,
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
