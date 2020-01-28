import { Component, Input } from '@angular/core';
import { Product } from '@ordercloud/angular-sdk';
import { ProductService } from '@app-seller/shared/services/product/product.service';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
import { MarketPlaceProduct, MarketPlaceProductImage } from '@app-seller/shared/models/MarketPlaceProduct.interface';
import { ReplaceHostUrls } from '@app-seller/shared/services/product/product-image.helper';

@Component({
  selector: 'app-product-view',
  templateUrl: './product-view.component.html',
  styleUrls: ['./product-view.component.scss'],
})
export class ProductViewComponent {
  images: MarketPlaceProductImage[] = [];
  _marketPlaceProduct: MarketPlaceProduct;

  @Input()
  set orderCloudProduct(product: Product) {
    if (Object.keys(product).length) {
      this.handleSelectedProductChange(product);
    }
  }

  constructor(private productService: ProductService, private currentUserService: CurrentUserService) {}

  private async handleSelectedProductChange(product: Product): Promise<void> {
    const marketPlaceProduct = await this.productService.getMarketPlaceProductByID(product.ID);
    this.refreshProductData(marketPlaceProduct);
  }

  refreshProductData(product: MarketPlaceProduct) {
    this._marketPlaceProduct = product;
    this.images = ReplaceHostUrls(product);
  }
}
