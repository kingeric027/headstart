import { Component, Input } from '@angular/core';
import { OcSupplierService, Product, Supplier } from '@ordercloud/angular-sdk';
import { ProductService } from '@app-seller/products/product.service';
import { ReplaceHostUrls } from '@app-seller/products/product-image.helper';
import { SuperMarketplaceProduct, ProductImage, MarketplaceSDK } from 'marketplace-javascript-sdk';

@Component({
  selector: 'app-product-view',
  templateUrl: './product-view.component.html',
  styleUrls: ['./product-view.component.scss'],
})
export class ProductViewComponent {
  images: ProductImage[] = [];
  _superMarketplaceProduct: SuperMarketplaceProduct;
  supplier: Supplier;

  @Input()
  set orderCloudProduct(product: Product) {
    if (Object.keys(product).length) {
      this.handleSelectedProductChange(product);
    }
  }

  constructor(private productService: ProductService, private ocSupplierService: OcSupplierService) {}

  refreshProductData(product: SuperMarketplaceProduct) {
    this._superMarketplaceProduct = product;
    this.images = ReplaceHostUrls(product.Product);
  }

  private async handleSelectedProductChange(product: Product): Promise<void> {
    const superMarketplaceProduct = await MarketplaceSDK.Products.Get(product.ID);
    this.supplier = await this.ocSupplierService.Get(superMarketplaceProduct.Product.OwnerID).toPromise();
    this.refreshProductData(superMarketplaceProduct);
  }
}
