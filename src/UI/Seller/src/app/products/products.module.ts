import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';

import { ProductsRoutingModule } from '@app-seller/products/products-routing.module';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { ProductTableComponent } from './product-table/product-table.component';
import { ProductEditComponent } from './product-edit/product-edit.component';
import { ProductVisibilityAssignments } from './product-visibility-assignments/product-visibility-assignments.component';
import { ProductViewComponent } from './product-view/product-view.component';
import { ProductTaxCodeSelect } from './product-tax-code-select/product-tax-code-select.component';
import { ProductTaxCodeSelectDropdown } from './product-tax-code-select-dropdown/product-tax-code-select-dropdown.component';

@NgModule({
  imports: [SharedModule, ProductsRoutingModule, PerfectScrollbarModule],
  declarations: [
    ProductTableComponent,
    ProductEditComponent,
    ProductViewComponent,
    ProductVisibilityAssignments,
    ProductTaxCodeSelect,
    ProductTaxCodeSelectDropdown,
  ],
})
export class ProductsModule {}
