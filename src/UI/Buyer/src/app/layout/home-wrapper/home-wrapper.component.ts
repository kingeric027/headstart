import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ListBuyerProduct } from '@ordercloud/angular-sdk';
import { FavoriteProductsService } from '@app-buyer/shared/services/favorites/favorites.service';
import { BuildQtyLimits } from '@app-buyer/shared';
import { QuantityLimits } from '@app-buyer/shared/models/quantity-limits';

@Component({
  selector: 'home-page-wrapper',
  templateUrl: './home-wrapper.component.html',
  styleUrls: ['./home-wrapper.component.scss'],
})
export class HomePageWrapperComponent implements OnInit {
  featuredProducts: ListBuyerProduct;
  favoriteProductIDs: string[];
  quantityLimits: QuantityLimits[];

  constructor(private activatedRoute: ActivatedRoute, private favoriteProducts: FavoriteProductsService, private router: Router) {}

  ngOnInit() {
    this.featuredProducts = this.activatedRoute.snapshot.data.featuredProducts;
    debugger;
    this.favoriteProductIDs = this.favoriteProducts.getFavorites();
    this.quantityLimits = this.featuredProducts.Items.map((p) => BuildQtyLimits(p));
  }

  toProductDetails(productID: string) {
    this.router.navigateByUrl(`/products/${productID}`);
  }
}
