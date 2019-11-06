import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';
import { SuppliersRoutingModule } from './suppliers-routing.module';
import { SupplierLocationListComponent } from './components/locations/supplier-location-list/supplier-location-list.component';
import { SupplierLocationCreateComponent } from './components/locations/supplier-location-create/supplier-location-create.component';
import { SupplierLocationDetailsComponent } from './components/locations/supplier-location-details/supplier-location-details.component';
import { SupplierDetailsComponent } from './components/suppliers/supplier-details/supplier-details.component';
import { SupplierListComponent } from './components/suppliers/supplier-list/supplier-list.component';
import { SupplierCreateComponent } from './components/suppliers/supplier-create/supplier-create.component';
import { SupplierUserListComponent } from './components/users/supplier-user-list/supplier-user-list.component';
import { SupplierUserCreateComponent } from './components/users/supplier-user-create/supplier-user-create.component';
import { SupplierUserDetailsComponent } from './components/users/supplier-user-details/supplier-user-details.component';

@NgModule({
  imports: [SharedModule, SuppliersRoutingModule],
  declarations: [
    SupplierLocationListComponent,
    SupplierLocationCreateComponent,
    SupplierLocationDetailsComponent,
    SupplierDetailsComponent,
    SupplierListComponent,
    SupplierCreateComponent,
    SupplierUserListComponent,
    SupplierUserCreateComponent,
    SupplierUserDetailsComponent,
  ],
})
export class SuppliersModule {}
