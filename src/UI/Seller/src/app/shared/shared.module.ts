// angular
import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import {
  NgbPaginationModule,
  NgbTabsetModule,
  NgbPopoverModule,
  NgbDropdownModule,
  NgbModalModule,
} from '@ng-bootstrap/ng-bootstrap';

// 3rd party UI
import { TreeModule } from 'angular-tree-component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

import { SharedRoutingModule } from '@app-seller/shared/shared-routing.module';
import { SearchComponent } from '@app-seller/shared/components/search/search.component';
import { SortColumnComponent } from '@app-seller/shared/components/sort-column/sort-column.component';
import { CarouselSlideDisplayComponent } from '@app-seller/shared/components/carousel-slide-display/carousel-slide-display.component';
import { UserFormComponent } from '@app-seller/shared/components/user-form/user-form.component';
import { AddressFormComponent } from '@app-seller/shared/components/address-form/address-form.component';
import { CategoryTableComponent } from './components/category-table/category-table.component';
import { CategoryFormComponent } from './components/category-form/category-form.component';
import { CategoryDetailsComponent } from './components/category-details/category-details.component';
import { ProductImagesComponent } from './components/product-images/product-images.component';
import { ProductFormComponent } from './components/products-form/product-form.component';
import { ProductService } from './services/product/product.service';
import { SupplierService } from './services/supplier/supplier.service';
import { ResourceTableComponent } from './components/resource-table/resource-table.component';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { ResourceSelectDropdown } from './components/resource-select-dropdown/resource-select-dropdown.component';
import { SummaryResourceDisplay } from './components/summary-resource-display/summary-resource-display.component';
import { DeleteConfirmModal } from './components/delete-confirm-modal/delete-confirm-modal.component';
import { ResourceEditComponent } from './components/resource-edit/resource-edit.component';

@NgModule({
  imports: [
    SharedRoutingModule,
    // angular
    CommonModule,
    HttpClientModule,
    ReactiveFormsModule,

    // 3rd party UI
    TreeModule,
    FontAwesomeModule,
    FormsModule,
    PerfectScrollbarModule,
    NgbPopoverModule,
    NgbDropdownModule,
    NgbPaginationModule,
    NgbTabsetModule,
    NgbModalModule,
  ],
  exports: [
    // angular
    CommonModule,
    HttpClientModule,
    ReactiveFormsModule,

    // 3rd party UI
    TreeModule,
    FontAwesomeModule,
    NgbPaginationModule,
    NgbTabsetModule,
    NgbDropdownModule,

    // app components
    SearchComponent,
    SortColumnComponent,
    CarouselSlideDisplayComponent,
    UserFormComponent,
    AddressFormComponent,
    CategoryTableComponent,
    CategoryFormComponent,
    CategoryDetailsComponent,
    ResourceSelectDropdown,
    ProductImagesComponent,
    ProductFormComponent,
    ResourceTableComponent,
    SummaryResourceDisplay,
    ResourceEditComponent,
  ],
  declarations: [
    SearchComponent,
    SortColumnComponent,
    CarouselSlideDisplayComponent,
    UserFormComponent,
    AddressFormComponent,
    CategoryTableComponent,
    CategoryFormComponent,
    CategoryDetailsComponent,
    ProductImagesComponent,
    ResourceSelectDropdown,
    ProductFormComponent,
    ResourceTableComponent,
    DeleteConfirmModal,
    SummaryResourceDisplay,
    ResourceEditComponent,
  ],
})
export class SharedModule {
  static forRoot(): ModuleWithProviders {
    return {
      ngModule: SharedModule,
      providers: [ProductService, SupplierService],
    };
  }
}
