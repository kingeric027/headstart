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
import { ReturnForm } from './components/return-form/return-form.component';
import { ResourceEditComponent } from './components/resource-edit/resource-edit.component';
import { FullResourceTableComponent } from './components/full-resource-table/full-resource-table.component';
import { QuillModule } from 'ngx-quill';
import { ReactiveQuillComponent } from './components/reactive-quill-editor/reactive-quill-editor.component';
import { FormControlErrorDirective } from './directives/form-control-errors.directive';
import { RequestStatus } from './components/request-status/request-status.component';
import { DragDirective } from './directives/dragDrop.directive';
import { ConfirmModal } from './components/confirm-modal/confirm-modal.component';
import { PhoneFormatPipe } from './pipes/phone-format.pipe';
import { YesNoFormatPipe } from './pipes/yes-no-format.pipe';
import { UserGroupAssignments } from './components/user-group-assignments/user-group-assignments.component';
import { LocationIDInputDirective } from './directives/location-id-input.directive';
import { ActionMenuComponent } from './components/action-menu/action-menu.component';
import { TranslateModule } from '@ngx-translate/core';
import { SafeHTMLPipe } from './pipes/safe-html.pipe';
import { CmsAdminModule } from '@ordercloud/angular-cms-components';
import { ResourceBreadcrumbsComponent } from './components/resource-breadcrumbs/resource-breadcrumbs.component';

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

    // Quill
    QuillModule.forRoot(),

    // OrderCloud CMS Components
    CmsAdminModule,
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

    // OrderCloud CMS Components
    CmsAdminModule,

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
    LocationIDInputDirective,
    DragDirective,
    DeleteConfirmModal,
    ReturnForm,
    PhoneFormatPipe,
    YesNoFormatPipe,
    SafeHTMLPipe,
    ActionMenuComponent,
    ConfirmModal,
    UserGroupAssignments,
    TranslateModule,
    ResourceBreadcrumbsComponent,
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
    ReturnForm,
    SummaryResourceDisplay,
    FullResourceTableComponent,
    ResourceEditComponent,
    ReactiveQuillComponent,
    FormControlErrorDirective,
    LocationIDInputDirective,
    DragDirective,
    ConfirmModal,
    ActionMenuComponent,
    UserGroupAssignments,
    // Pipes
    PhoneFormatPipe,
    YesNoFormatPipe,
    ResourceBreadcrumbsComponent,
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
