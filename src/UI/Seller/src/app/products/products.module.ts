import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';

import { ProductsRoutingModule } from '@app-seller/products/products-routing.module';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { ProductTableComponent } from './components/product-table/product-table.component';
import { ProductEditComponent } from './components/product-edit/product-edit.component';
import { ProductViewComponent } from './components/product-view/product-view.component';
import { ProductVisibilityAssignments } from './components/product-visibility-assignments/product-visibility-assignments.component';
import { ProductTaxCodeSelect } from './components/product-tax-code-select/product-tax-code-select.component';
import { ProductTaxCodeSelectDropdown } from './components/product-tax-code-select-dropdown/product-tax-code-select-dropdown.component';

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
