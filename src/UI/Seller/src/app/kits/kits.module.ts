import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';

import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { KitsRoutingModule } from './kits-routing.module';
import { KitsTableComponent } from './components/kits-table/kits-table.component';
import { KitsEditComponent } from './components/kits-edit/kits-edit.component';
import { NgbDatepickerModule, NgbPopoverModule } from '@ng-bootstrap/ng-bootstrap';
import { ProductVisibilityAssignments } from '../products/components/buyer-visibility/product-visibility-assignments/product-visibility-assignments.component';
import { BuyerVisibilityConfiguration } from '../products/components/buyer-visibility/buyer-visibility-configuration/buyer-visibility-configuration.component';
@NgModule({
    imports: [SharedModule, KitsRoutingModule, PerfectScrollbarModule, NgbDatepickerModule, NgbPopoverModule, ProductVisibilityAssignments, BuyerVisibilityConfiguration],
    declarations: [KitsTableComponent, KitsEditComponent],
})
export class KitsModule { }
