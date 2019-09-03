import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ListBuyerProduct } from '@ordercloud/angular-sdk';
import { FavoriteProductsService } from '@app-buyer/shared/services/favorites/favorites.service';
import { BuildQtyLimits } from '@app-buyer/shared';
import { QuantityLimits } from '@app-buyer/shared/models/quantity-limits';
import { NavigatorService } from '@app-buyer/shared/services/navigator/navigator.service';

@Component({
  selector: 'home-page-wrapper',
  templateUrl: './home-wrapper.component.html',
  styleUrls: ['./home-wrapper.component.scss'],
})
export class HomePageWrapperComponent implements OnInit {
  featuredProducts: ListBuyerProduct;
  favoriteProductIDs: string[];
  quantityLimits: QuantityLimits[];

  constructor(
    private activatedRoute: ActivatedRoute,
    private favoriteProducts: FavoriteProductsService,
    protected navigator: NavigatorService // used in template
  ) {}

  ngOnInit() {
    this.featuredProducts = this.activatedRoute.snapshot.data.featuredProducts;
    this.favoriteProductIDs = this.favoriteProducts.getFavorites();
    this.quantityLimits = this.featuredProducts.Items.map((p) => BuildQtyLimits(p));
  }
}
