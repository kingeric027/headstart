import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { RouterModule } from '@angular/router';

import { HeaderComponent } from '@app-buyer/layout/header/header.component';
import { FooterComponent } from '@app-buyer/layout/footer/footer.component';

import { SharedModule } from '@app-buyer/shared';
import { HomeComponent } from '@app-buyer/layout/home/home.component';

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [RouterModule, SharedModule],
  exports: [HeaderComponent, FooterComponent, HomeComponent],
  declarations: [HeaderComponent, FooterComponent, HomeComponent],
})
export class LayoutModule {}
