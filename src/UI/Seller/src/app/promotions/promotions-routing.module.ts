// core services
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PromotionListComponent } from './components/promotions/promotion-list/promotion-list.component';

const routes: Routes = [
  { path: '', component: PromotionListComponent },
  { path: 'new', component: PromotionListComponent },
  { path: ':promotionID', component: PromotionListComponent },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PromotionsRoutingModule {}
