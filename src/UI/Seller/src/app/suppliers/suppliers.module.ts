import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';
import { SuppliersRoutingModule } from './suppliers-routing.module';
import { SupplierLocationListComponent } from './components/locations/supplier-location-list/supplier-location-list.component';
import { SupplierLocationCreateComponent } from './components/locations/supplier-location-create/supplier-location-create.component';
import { SupplierLocationDetailsComponent } from './components/locations/supplier-location-details/supplier-location-details.component';
import { SupplierTableComponent } from './components/suppliers/supplier-table/supplier-table.component';
import { SupplierCreateComponent } from './components/suppliers/supplier-create/supplier-create.component';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { SupplierUserTableComponent } from './components/users/user-table/supplier-user-table.component';

@NgModule({
  imports: [SharedModule, SuppliersRoutingModule, PerfectScrollbarModule],
  declarations: [
    SupplierLocationListComponent,
    SupplierLocationCreateComponent,
    SupplierLocationDetailsComponent,
    SupplierTableComponent,
    SupplierCreateComponent,
    SupplierUserTableComponent,
  ],
})
export class SuppliersModule {}
