// core services
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProductCreateComponent } from './components/products/product-create/product-create.component';
import { CategoryListComponent } from './components/categories/category-list/category-list.component';
import { CategoryCreateComponent } from './components/categories/category-create/category-create.component';
import { CategoryDetailsComponent } from '@app-seller/shared/components/category-details/category-details.component';
import { ProductListComponent } from './components/products/product-list/product-list.component';
import { ProductDetailsComponent } from './components/products/product-details/product-details.component';

const routes: Routes = [
  { path: '', component: ProductListComponent },
  { path: 'new', component: ProductCreateComponent },
  { path: ':productID', component: ProductDetailsComponent },
  { path: '/categories', component: CategoryListComponent },
  { path: '/categories/new', component: CategoryCreateComponent },
  { path: '/categories/:categoryID', component: CategoryDetailsComponent },
  { path: '/promotions', component: CategoryListComponent },
  { path: '/promotions/new', component: CategoryCreateComponent },
  { path: '/promotions/:promotionID', component: CategoryDetailsComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ProductsRoutingModule {}
