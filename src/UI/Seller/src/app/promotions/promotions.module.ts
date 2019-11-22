import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';

import { PromotionListComponent } from './components/promotions/promotion-list/promotion-list.component';
import { PromotionCreateComponent } from './components/promotions/promotion-create/promotion-create.component';
import { PromotionDetailsComponent } from './components/promotions/promotion-details/promotion-details.component';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { PromotionsRoutingModule } from './promotions-routing.module';

@NgModule({
  imports: [SharedModule, PromotionsRoutingModule, PerfectScrollbarModule],
  declarations: [PromotionListComponent, PromotionCreateComponent, PromotionDetailsComponent],
})
export class PromotionsModule {}
