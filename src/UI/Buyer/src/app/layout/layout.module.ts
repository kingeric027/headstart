import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { RouterModule } from '@angular/router';

import { OCMAppHeader } from '@app-buyer/layout/header/header.component';
import { FooterComponent } from '@app-buyer/layout/footer/footer.component';

import { SharedModule } from '@app-buyer/shared';
import { HomePageWrapperComponent } from './home-wrapper/home-wrapper.component';
import { FeaturedProductsResolver } from './resolves/features-products.resolve';
import { HeaderWrapperComponent } from './header-wrapper/header-wrapper.component';

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [RouterModule, SharedModule],
  exports: [OCMAppHeader, FooterComponent, HeaderWrapperComponent],
  providers: [FeaturedProductsResolver],
  declarations: [OCMAppHeader, FooterComponent, HomePageWrapperComponent, HeaderWrapperComponent],
})
export class LayoutModule {}
