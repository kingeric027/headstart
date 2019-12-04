import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';

import { ProductsRoutingModule } from '@app-seller/products/products-routing.module';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { ProductTableComponent } from './components/products/product-table/product-table.component';

@NgModule({
  imports: [SharedModule, ProductsRoutingModule, PerfectScrollbarModule],
  declarations: [ProductTableComponent],
})
export class ProductsModule {}
