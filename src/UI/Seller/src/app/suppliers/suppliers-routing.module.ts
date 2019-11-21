// core services
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SupplierTableComponent } from './components/suppliers/supplier-table/supplier-table.component';
import { SupplierLocationListComponent } from './components/locations/supplier-location-list/supplier-location-list.component';
import { SupplierLocationCreateComponent } from './components/locations/supplier-location-create/supplier-location-create.component';
import { SupplierLocationDetailsComponent } from './components/locations/supplier-location-details/supplier-location-details.component';
import { SupplierUserTableComponent } from './components/users/user-table/supplier-user-table.component';

const routes: Routes = [
  { path: '', component: SupplierTableComponent },
  { path: 'new', component: SupplierTableComponent },
  { path: ':supplierID', component: SupplierTableComponent },
  { path: ':supplierID/users', component: SupplierUserTableComponent },
  { path: ':supplierID/users/new', component: SupplierUserTableComponent },
  { path: ':supplierID/users/:userID', component: SupplierUserTableComponent },
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
