// core services
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CategoryListComponent } from './components/categories/category-list/category-list.component';
import { CategoryCreateComponent } from './components/categories/category-create/category-create.component';
import { CategoryDetailsComponent } from '@app-seller/shared/components/category-details/category-details.component';

const routes: Routes = [
  { path: '', component: CategoryListComponent },
  { path: 'new', component: CategoryCreateComponent },
  { path: ':categoryID', component: CategoryDetailsComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class CategoriesRoutingModule {}
