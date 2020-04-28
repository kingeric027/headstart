// core services
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProductTableComponent } from './components/product-table/product-table.component';

const routes: Routes = [
  { path: '', component: ProductTableComponent, pathMatch: 'prefix' },
  { path: 'new', component: ProductTableComponent, pathMatch: 'full' },
  { path: ':productID', component: ProductTableComponent, pathMatch: 'full' },
  { path: ':productID/description', component: ProductTableComponent, pathMatch: 'full' },
  { path: ':productID/filters', component: ProductTableComponent, pathMatch: 'full' },
  { path: ':productID/variants', component: ProductTableComponent, pathMatch: 'full' },
  { path: ':productID/images-and-documents', component: ProductTableComponent, pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ProductsRoutingModule {}
