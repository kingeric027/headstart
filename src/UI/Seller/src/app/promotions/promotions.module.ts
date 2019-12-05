import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';

import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { PromotionsRoutingModule } from './promotions-routing.module';
import { PromotionTableComponent } from './components/promotions/promotion-table/promotion-table.component';

@NgModule({
  imports: [SharedModule, PromotionsRoutingModule, PerfectScrollbarModule],
  declarations: [PromotionTableComponent],
})
export class PromotionsModule {}
