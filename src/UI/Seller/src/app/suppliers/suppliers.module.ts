import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';
import { SuppliersRoutingModule } from './suppliers-routing.module';
import { SupplierTableComponent } from './components/suppliers/supplier-table/supplier-table.component';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { SupplierUserTableComponent } from './components/users/user-table/supplier-user-table.component';
import { SupplierLocationTableComponent } from './components/locations/supplier-location-table/supplier-location-table.component';

@NgModule({
  imports: [SharedModule, SuppliersRoutingModule, PerfectScrollbarModule],
  declarations: [SupplierLocationTableComponent, SupplierTableComponent, SupplierUserTableComponent],
})
export class SuppliersModule {}
