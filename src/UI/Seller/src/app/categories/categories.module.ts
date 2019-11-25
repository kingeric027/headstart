import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';

import { ProductsRoutingModule } from '@app-seller/products/products-routing.module';
import { CategoryListComponent } from './components/categories/category-list/category-list.component';
import { CategoryNewComponent } from './components/categories/category-new/category-new.component';
import { CategoryCreateComponent } from './components/categories/category-create/category-create.component';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { CategoriesRoutingModule } from './categories-routing.module';

@NgModule({
  imports: [SharedModule, CategoriesRoutingModule, PerfectScrollbarModule],
  declarations: [CategoryListComponent, CategoryNewComponent, CategoryCreateComponent],
})
export class CategoriesModule {}
