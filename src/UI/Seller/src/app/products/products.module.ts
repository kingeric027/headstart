import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';

import { ProductsRoutingModule } from '@app-seller/products/products-routing.module';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { ProductTableComponent } from './components/product-table/product-table.component';
import { ProductEditComponent } from './components/product-edit/product-edit.component';
import { ProductVisibilityAssignments } from './components/buyer-visibility/product-visibility-assignments/product-visibility-assignments.component';
import { VariablePrintConfiguration } from './components/buyer-visibility/variable-print-configuration/variable-print-configuration.component';
import { ProductTaxCodeSelect } from './components/product-tax-code-select/product-tax-code-select.component';
import { ProductTaxCodeSelectDropdown } from './components/product-tax-code-select-dropdown/product-tax-code-select-dropdown.component';
import { ProductVariations } from './components/product-variations/product-variations.component';
import { ProductFilters } from './components/product-filters/product-filters.component';
import { BuyerVisibilityConfiguration } from './components/buyer-visibility/buyer-visibility-configuration/buyer-visibility-configuration.component';
import { ProductCategoryAssignment } from './components/buyer-visibility/product-category-assignment/product-category-assignment.component';
import { ProductPricingComponent } from './components/product-pricing/product-pricing.component';
import { PriceBreakEditor } from './components/price-break-editor/price-break-editor.component';
import { NgbTabset, NgbModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  imports: [SharedModule, ProductsRoutingModule, PerfectScrollbarModule, NgbModule],
  declarations: [
    ProductTableComponent,
    ProductEditComponent,
    ProductPricingComponent,
    PriceBreakEditor,
    ProductVisibilityAssignments,
    BuyerVisibilityConfiguration,
    VariablePrintConfiguration,
    ProductCategoryAssignment,
    ProductTaxCodeSelect,
    ProductTaxCodeSelectDropdown,
    ProductVariations,
    ProductFilters,
  ],
})
export class ProductsModule {}
