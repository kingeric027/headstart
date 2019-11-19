// angular
import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { NgbPaginationModule, NgbTabsetModule } from '@ng-bootstrap/ng-bootstrap';

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
    NgbPaginationModule.forRoot(),
    NgbTabsetModule.forRoot(),
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

    // app components
    SearchComponent,
    SortColumnComponent,
    CarouselSlideDisplayComponent,
    UserFormComponent,
    AddressFormComponent,
    CategoryTableComponent,
    CategoryFormComponent,
    CategoryDetailsComponent,
    ProductImagesComponent,
    ProductFormComponent,
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
    ProductFormComponent,
  ],
})
export class SharedModule {
  static forRoot(): ModuleWithProviders {
    return {
      ngModule: SharedModule,
      providers: [ProductService],
    };
  }
}
