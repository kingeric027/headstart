import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';

import { ProductsRoutingModule } from '@app-seller/products/products-routing.module';
import { ProductListComponent } from './components/products/product-list/product-list.component';
import { ProductDetailsComponent } from './components/products/product-details/product-details.component';
import { ProductCreateComponent } from './components/products/product-create/product-create.component';
import { CategoryListComponent } from './components/categories/category-list/category-list.component';
import { CategoryNewComponent } from './components/categories/category-new/category-new.component';
import { CategoryCreateComponent } from './components/categories/category-create/category-create.component';
import { PromotionListComponent } from './components/promotions/promotion-list/promotion-list.component';
import { PromotionCreateComponent } from './components/promotions/promotion-create/promotion-create.component';
import { PromotionDetailsComponent } from './components/promotions/promotion-details/promotion-details.component';

@NgModule({
  imports: [SharedModule, ProductsRoutingModule],
  declarations: [
    ProductListComponent,
    ProductDetailsComponent,
    ProductCreateComponent,
    CategoryListComponent,
    CategoryNewComponent,
    CategoryCreateComponent,
    PromotionListComponent,
    PromotionCreateComponent,
    PromotionDetailsComponent,
  ],
})
export class ProductsModule {}
