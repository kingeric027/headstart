import { Component, OnInit } from '@angular/core';
import {
  OcMeService,
  BuyerSpec,
  BuyerProduct,
  LineItem,
} from '@ordercloud/angular-sdk';
import { ActivatedRoute } from '@angular/router';
import { CartService } from '@app-buyer/shared';
import { FavoriteProductsService } from '@app-buyer/shared/services/favorites/favorites.service';

@Component({
  selector: 'product-detail-wrapper',
  templateUrl: './product-detail-wrapper.component.html',
  styleUrls: ['./product-detail-wrapper.component.scss'],
})
export class ProductDetailWrapperComponent implements OnInit {
  specs: BuyerSpec[] = [];
  product: BuyerProduct;

  constructor(
    private ocMeService: OcMeService,
    private activatedRoute: ActivatedRoute,
    private cartService: CartService,
    protected favoriteProductService: FavoriteProductsService
  ) {}

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(async (params) => {
      await this.getProductData(params.productID);
    });
  }

  async getProductData(productID: string): Promise<void> {
    if (!productID) return;
    const product = this.ocMeService.GetProduct(productID).toPromise();
    const specs = this.listSpecs(productID);
    [this.product, this.specs] = await Promise.all([product, specs]);
  }

  async listSpecs(productID: string): Promise<BuyerSpec[]> {
    const specs = await this.ocMeService.ListSpecs(productID).toPromise();
    const details = specs.Items.map((spec) => {
      return this.ocMeService.GetSpec(productID, spec.ID).toPromise();
    });
    return await Promise.all(details);
  }

  addToCart(li: LineItem) {
    this.cartService.addToCart(li);
  }
}
