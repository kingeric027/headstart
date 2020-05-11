import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';

import { ProductsRoutingModule } from '@app-seller/products/products-routing.module';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { ProductTableComponent } from './components/product-table/product-table.component';
import { ProductEditComponent } from './components/product-edit/product-edit.component';
import { ProductVisibilityAssignments } from './components/product-visibility-assignments/product-visibility-assignments.component';
import { ProductTaxCodeSelect } from './components/product-tax-code-select/product-tax-code-select.component';
import { ProductTaxCodeSelectDropdown } from './components/product-tax-code-select-dropdown/product-tax-code-select-dropdown.component';
import { ProductVariations } from './components/product-variations/product-variations.component';
import { ProductFilters } from './components/product-filters/product-filters.component';

@NgModule({
  imports: [SharedModule, ProductsRoutingModule, PerfectScrollbarModule],
  declarations: [
    ProductTableComponent,
    ProductEditComponent,
    ProductVisibilityAssignments,
    ProductTaxCodeSelect,
    ProductTaxCodeSelectDropdown,
    ProductVariations,
    ProductFilters,
  ],
})
export class ProductsModule {}
