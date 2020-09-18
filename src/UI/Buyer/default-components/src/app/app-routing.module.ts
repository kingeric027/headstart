import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MarketplaceRoutes } from 'marketplace';

@NgModule({
  imports: [RouterModule.forRoot(MarketplaceRoutes)],
  exports: [RouterModule],
})
export class AppRoutingModule { }
