import { Component, Input } from '@angular/core';
import { MarketPlaceProduct, MarketPlaceProductImage } from '@app-seller/shared/models/MarketPlaceProduct.interface';
import { ReplaceHostUrls } from '@app-seller/shared/services/product/product-image.helper';
import { ProductService } from '@app-seller/shared/services/product/product.service';
import { OcSupplierService, Product, Supplier } from '@ordercloud/angular-sdk';

@Component({
  selector: 'app-product-view',
  templateUrl: './product-view.component.html',
  styleUrls: ['./product-view.component.scss'],
})
export class ProductViewComponent {
  images: MarketPlaceProductImage[] = [];
  _marketPlaceProduct: MarketPlaceProduct;
  supplier: Supplier;

  @Input()
  set orderCloudProduct(product: Product) {
    if (Object.keys(product).length) {
      this.handleSelectedProductChange(product);
    }
  }

  constructor(private productService: ProductService, private ocSupplierService: OcSupplierService) { }

  private async handleSelectedProductChange(product: Product): Promise<void> {
    const marketPlaceProduct = await this.productService.getMarketPlaceProductByID(product.ID);
    this.supplier = await this.ocSupplierService.Get(marketPlaceProduct['OwnerID']).toPromise();
    this.refreshProductData(marketPlaceProduct);
  }

  refreshProductData(product: MarketPlaceProduct) {
    this._marketPlaceProduct = product;
    this.images = ReplaceHostUrls(product);
  }
}
