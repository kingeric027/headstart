import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';

import { ProductsRoutingModule } from '@app-seller/products/products-routing.module';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { ProductTableComponent } from './product-table/product-table.component';
import { ProductEditComponent } from './product-edit/product-edit.component';
import { ProductCatalogAssignments } from './product-catalog-assignments/product-catalog-assignments.component';

@NgModule({
  imports: [SharedModule, ProductsRoutingModule, PerfectScrollbarModule],
  declarations: [ProductTableComponent, ProductEditComponent, ProductCatalogAssignments],
})
export class ProductsModule {}
