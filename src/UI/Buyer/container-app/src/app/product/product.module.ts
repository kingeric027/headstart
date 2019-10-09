import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { SharedModule } from 'src/app/shared';
import { ProductsRoutingModule } from 'src/app/product/product-routing.module';
import {
  MeListProductResolver,
  MeProductResolver,
  MeListSpecsResolver,
  MeSpecsResolver,
  MeListCategoriesResolver,
  MeListRelatedProductsResolver,
} from './resolves/me.product.resolve';
import { OcmDefaultComponentsModule } from 'ocm-components';
import { SpecFormAddToCartComponent } from './components/spec-form/spec-form-add-to-cart/spec-form-add-to-cart.component';
import { SpecFormButtonComponent } from './components/spec-form/spec-form-button/spec-form-button.component';
import { SpecFormCheckboxComponent } from './components/spec-form/spec-form-checkbox/spec-form-checkbox.component';
import { SpecFormInputComponent } from './components/spec-form/spec-form-input/spec-form-input.component';
import { SpecFormNumberComponent } from './components/spec-form/spec-form-number/spec-form-number.component';
import { SpecFormSelectComponent } from './components/spec-form/spec-form-select/spec-form-select.component';
import { SpecFormTextAreaComponent } from './components/spec-form/spec-form-textarea/spec-form-textarea.component';
import { SpecFieldDirective } from './components/spec-form/spec-field.directive';
import { SpecFormComponent } from './components/spec-form/spec-form.component';
import { ProductDetailWrapperComponent } from './components/product-detail-wrapper/product-detail-wrapper.component';
import { ProductListWrapperComponent } from './components/product-list-wrapper/product-list-wrapper.component';

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [SharedModule, ProductsRoutingModule, OcmDefaultComponentsModule],
  declarations: [
    ProductDetailWrapperComponent,
    ProductListWrapperComponent,
    SpecFormAddToCartComponent,
    SpecFormButtonComponent,
    SpecFormCheckboxComponent,
    SpecFormInputComponent,
    SpecFormNumberComponent,
    SpecFormSelectComponent,
    SpecFormTextAreaComponent,
    SpecFieldDirective,
    SpecFormComponent,
  ],
  providers: [
    MeListProductResolver,
    MeProductResolver,
    MeListSpecsResolver,
    MeSpecsResolver,
    MeListCategoriesResolver,
    MeListRelatedProductsResolver,
  ],
})
export class ProductsModule {}
