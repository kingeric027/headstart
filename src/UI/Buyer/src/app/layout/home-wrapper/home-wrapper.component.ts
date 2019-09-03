import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ListBuyerProduct } from '@ordercloud/angular-sdk';
import { BuildQtyLimits } from '@app-buyer/shared';
import { QuantityLimits } from '@app-buyer/shared/models/quantity-limits';
import { NavigatorService } from '@app-buyer/shared/services/navigator/navigator.service';
import { CurrentUserService } from '@app-buyer/shared/services/current-user/current-user.service';

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
    private currentUser: CurrentUserService,
    protected navigator: NavigatorService // used in template
  ) {}

  ngOnInit() {
    this.featuredProducts = this.activatedRoute.snapshot.data.featuredProducts;
    this.favoriteProductIDs = this.currentUser.favoriteProductIDs;
    this.quantityLimits = this.featuredProducts.Items.map((p) => BuildQtyLimits(p));
  }

  setFavoriteValue(isFav: boolean, productID: string) {
    this.currentUser.setIsFavoriteProduct(isFav, productID);
  }
}
