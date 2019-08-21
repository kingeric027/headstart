import { Component, OnInit } from '@angular/core';
import { BuyerProduct, LineItem, ListSpec } from '@ordercloud/angular-sdk';
import { ActivatedRoute } from '@angular/router';
import { CartService, BuildQtyLimits } from '@app-buyer/shared';
import { FavoriteProductsService } from '@app-buyer/shared/services/favorites/favorites.service';
import { QuantityLimits } from '@app-buyer/shared/models/quantity-limits';

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
    private cartService: CartService,
    protected favoriteProductService: FavoriteProductsService
  ) {}

  ngOnInit(): void {
    this.product = this.activatedRoute.snapshot.data.product;
    this.specs = this.activatedRoute.snapshot.data.specs || [];
    this.quantityLimits = BuildQtyLimits(this.product);
  }

  addToCart(li: LineItem) {
    this.cartService.addToCart(li);
  }
}
