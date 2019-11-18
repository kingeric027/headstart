// core services
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SupplierListComponent } from './components/suppliers/supplier-list/supplier-list.component';
import { SupplierCreateComponent } from './components/suppliers/supplier-create/supplier-create.component';
import { SupplierDetailsComponent } from './components/suppliers/supplier-details/supplier-details.component';
import { SupplierUserListComponent } from './components/users/supplier-user-list/supplier-user-list.component';
import { SupplierUserCreateComponent } from './components/users/supplier-user-create/supplier-user-create.component';
import { SupplierUserDetailsComponent } from './components/users/supplier-user-details/supplier-user-details.component';
import { SupplierLocationListComponent } from './components/locations/supplier-location-list/supplier-location-list.component';
import { SupplierLocationCreateComponent } from './components/locations/supplier-location-create/supplier-location-create.component';
import { SupplierLocationDetailsComponent } from './components/locations/supplier-location-details/supplier-location-details.component';

const routes: Routes = [
  { path: '', component: SupplierListComponent },
  { path: 'new', component: SupplierCreateComponent },
  { path: ':supplierID', component: SupplierDetailsComponent },
  { path: ':supplierID/users', component: SupplierUserListComponent },
  { path: ':supplierID/users/new', component: SupplierUserCreateComponent },
  { path: ':supplierID/users/:userID', component: SupplierUserDetailsComponent },
  { path: ':supplierID/locations', component: SupplierLocationListComponent },
  { path: ':supplierID/locations/new', component: SupplierLocationCreateComponent },
  {
    path: ':supplierID/locations/:locationID',
    component: SupplierLocationDetailsComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class SuppliersRoutingModule {}
