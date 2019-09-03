import { Component, OnInit } from '@angular/core';
import { BuyerProduct, ListSpec } from '@ordercloud/angular-sdk';
import { ActivatedRoute } from '@angular/router';
import { CartService, BuildQtyLimits } from '@app-buyer/shared';
import { QuantityLimits } from '@app-buyer/shared/models/quantity-limits';
import { CurrentUserService } from '@app-buyer/shared/services/current-user/current-user.service';

@Component({
  selector: 'product-detail-wrapper',
  templateUrl: './product-detail-wrapper.component.html',
  styleUrls: ['./product-detail-wrapper.component.scss'],
})
export class ProductDetailWrapperComponent implements OnInit {
  specs: ListSpec;
  product: BuyerProduct;
  quantityLimits: QuantityLimits;

  constructor(
    private activatedRoute: ActivatedRoute,
    protected cartService: CartService, // used in template
    protected currentUser: CurrentUserService
  ) {}

  ngOnInit(): void {
    this.product = this.activatedRoute.snapshot.data.product;
    this.specs = this.activatedRoute.snapshot.data.specs || [];
    this.quantityLimits = BuildQtyLimits(this.product);
  }

  isFavorite(productID: string): boolean {
    return this.currentUser.favoriteProductIDs.includes(productID);
  }

  async setIsFavoriteProduct(isFav: boolean, productID: string) {
    await this.currentUser.setIsFavoriteProduct(isFav, productID);
  }
}
