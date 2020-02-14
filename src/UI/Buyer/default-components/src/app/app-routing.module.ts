import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MarketplaceRoutes } from 'marketplace';

@NgModule({
  imports: [RouterModule.forRoot(MarketplaceRoutes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
