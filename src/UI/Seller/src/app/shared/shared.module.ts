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
  NgbDatepickerModule,
  NgbTooltipModule,
} from '@ng-bootstrap/ng-bootstrap';

// 3rd party UI
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

import { SharedRoutingModule } from '@app-seller/shared/shared-routing.module';
import { SearchComponent } from '@app-seller/shared/components/search/search.component';
import { SortColumnComponent } from '@app-seller/shared/components/sort-column/sort-column.component';
import { CarouselSlideDisplayComponent } from '@app-seller/shared/components/carousel-slide-display/carousel-slide-display.component';
import { UserFormComponent } from '@app-seller/shared/components/user-form/user-form.component';
import { AddressFormComponent } from '@app-seller/shared/components/address-form/address-form.component';
import { CategoryFormComponent } from './components/category-form/category-form.component';
import { CategoryDetailsComponent } from './components/category-details/category-details.component';
import { AddressSuggestionComponent } from './components/address-suggestion/address-suggestion.component';
import { ProductImagesComponent } from './components/product-images/product-images.component';
import { ProductFormComponent } from './components/products-form/product-form.component';
import { ResourceTableComponent } from './components/resource-table/resource-table.component';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { ResourceSelectDropdown } from './components/resource-select-dropdown/resource-select-dropdown.component';
import { SummaryResourceDisplay } from './components/summary-resource-display/summary-resource-display.component';
import { DeleteConfirmModal } from './components/delete-confirm-modal/delete-confirm-modal.component';
import { ResourceEditComponent } from './components/resource-edit/resource-edit.component';
import { FullResourceTableComponent } from './components/full-resource-table/full-resource-table.component';
import { QuillModule } from 'ngx-quill';
import { ReactiveQuillComponent } from './components/reactive-quill-editor/reactive-quill-editor.component';
import { FormControlErrorDirective } from './directives/form-control-errors.directive';
import { RequestStatus } from './components/request-status/request-status.component';
import { DragDirective } from './directives/dragDrop.directive';
import { ConfirmModal } from './components/confirm-modal/confirm-modal.component';
import { PhoneFormatPipe } from './pipes/phone-format.pipe';
import { UserPermissionsAssignments } from './components/user-permissions-assignments/user-permissions-assignments.component';

@NgModule({
  imports: [
    SharedRoutingModule,
    // angular
    CommonModule,
    HttpClientModule,
    ReactiveFormsModule,

    // 3rd party UI
    FontAwesomeModule,
    FormsModule,
    PerfectScrollbarModule,
    NgbPopoverModule,
    NgbDropdownModule,
    NgbPaginationModule,
    NgbTabsetModule,
    NgbModalModule,
    NgbDatepickerModule,
    NgbTooltipModule,
    QuillModule.forRoot(),
  ],
  exports: [
    // angular
    CommonModule,
    HttpClientModule,
    ReactiveFormsModule,

    // 3rd party UI
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
    CategoryFormComponent,
    CategoryDetailsComponent,
    AddressSuggestionComponent,
    ResourceSelectDropdown,
    ProductImagesComponent,
    ProductFormComponent,
    ResourceTableComponent,
    SummaryResourceDisplay,
    FullResourceTableComponent,
    ResourceEditComponent,
    QuillModule,
    ReactiveQuillComponent,
    FormControlErrorDirective,
    DragDirective,
    DeleteConfirmModal,
    PhoneFormatPipe,
    ConfirmModal,
    // Pipes
    UserPermissionsAssignments,
  ],
  declarations: [
    SearchComponent,
    SortColumnComponent,
    CarouselSlideDisplayComponent,
    UserFormComponent,
    AddressFormComponent,
    CategoryFormComponent,
    CategoryDetailsComponent,
    AddressSuggestionComponent,
    ProductImagesComponent,
    ResourceSelectDropdown,
    ProductFormComponent,
    ResourceTableComponent,
    RequestStatus,
    DeleteConfirmModal,
    SummaryResourceDisplay,
    FullResourceTableComponent,
    ResourceEditComponent,
    ReactiveQuillComponent,
    FormControlErrorDirective,
    DragDirective,
    ConfirmModal,
    // Pipes
    PhoneFormatPipe,
    UserPermissionsAssignments,
  ],
})
export class SharedModule {
  static forRoot(): ModuleWithProviders {
    return {
      ngModule: SharedModule,
      providers: [],
    };
  }
}
