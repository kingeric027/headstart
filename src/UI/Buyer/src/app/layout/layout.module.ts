import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { RouterModule } from '@angular/router';

import { FooterComponent } from 'src/app/layout/footer/footer.component';

import { SharedModule } from 'src/app/shared';
import { HomePageWrapperComponent } from './home-wrapper/home-wrapper.component';
import { FeaturedProductsResolver } from './resolves/features-products.resolve';
import { HeaderWrapperComponent } from './header-wrapper/header-wrapper.component';

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [RouterModule, SharedModule],
  exports: [FooterComponent, HeaderWrapperComponent],
  providers: [FeaturedProductsResolver],
  declarations: [FooterComponent, HomePageWrapperComponent, HeaderWrapperComponent],
})
export class LayoutModule {}
