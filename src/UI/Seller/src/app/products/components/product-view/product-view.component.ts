import { Component, Input } from '@angular/core';
import {
  MarketPlaceProductImage,
  SuperMarketplaceProduct,
} from '@app-seller/shared/models/MarketPlaceProduct.interface';
import { OcSupplierService, Product, Supplier } from '@ordercloud/angular-sdk';
import { MiddlewareAPIService } from '@app-seller/shared/services/middleware-api/middleware-api.service';
import { ProductService } from '@app-seller/products/product.service';
import { ReplaceHostUrls } from '@app-seller/products/product-image.helper';

@Component({
  selector: 'app-product-view',
  templateUrl: './product-view.component.html',
  styleUrls: ['./product-view.component.scss'],
})
export class ProductViewComponent {
  images: MarketPlaceProductImage[] = [];
  _superMarketplaceProduct: SuperMarketplaceProduct;
  supplier: Supplier;

  @Input()
  set orderCloudProduct(product: Product) {
    if (Object.keys(product).length) {
      this.handleSelectedProductChange(product);
    }
  }

  constructor(
    private productService: ProductService,
    private ocSupplierService: OcSupplierService,
    private middleware: MiddlewareAPIService
  ) {}

  refreshProductData(product: SuperMarketplaceProduct) {
    this._superMarketplaceProduct = product;
    this.images = ReplaceHostUrls(product.Product);
  }

  private async handleSelectedProductChange(product: Product): Promise<void> {
    const superMarketplaceProduct = await this.middleware.getSuperMarketplaceProductByID(product.ID);
    this.supplier = await this.ocSupplierService.Get(superMarketplaceProduct.Product.OwnerID).toPromise();
    this.refreshProductData(superMarketplaceProduct);
  }
}