// core services
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { StorefrontListComponent } from './components/storefront-list/storefront-list.component';

const routes: Routes = [{ path: '', component: StorefrontListComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class StorefrontsRoutingModule {}
