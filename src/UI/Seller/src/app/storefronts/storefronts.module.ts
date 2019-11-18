import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';
import { StorefrontListComponent } from './components/storefront-list/storefront-list.component';
import { StorefrontsRoutingModule } from './storefronts-routing.module';

@NgModule({
  imports: [SharedModule, StorefrontsRoutingModule],
  declarations: [StorefrontListComponent],
})
export class StorefrontsModule {}
