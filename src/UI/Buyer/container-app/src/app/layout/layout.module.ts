import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { RouterModule } from '@angular/router';

import { SharedModule } from 'src/app/shared';
import { HomePageWrapperComponent } from './components/home-wrapper.component';
import { FeaturedProductsResolver } from './resolves/features-products.resolve';
import { HeaderWrapperComponent } from './components/header-wrapper.component';
import { FooterWrapperComponent } from './components/footer-wrapper.component';

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [RouterModule, SharedModule],
  exports: [FooterWrapperComponent, HeaderWrapperComponent],
  providers: [FeaturedProductsResolver],
  declarations: [FooterWrapperComponent, HomePageWrapperComponent, HeaderWrapperComponent],
})
export class LayoutModule {}
