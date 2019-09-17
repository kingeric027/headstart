import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ListBuyerProduct } from '@ordercloud/angular-sdk';
import { BuildQtyLimits } from 'src/app/shared';
import { QuantityLimits } from 'src/app/shared/models/quantity-limits';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';

@Component({
  selector: 'home-page-wrapper',
  templateUrl: './home-wrapper.component.html',
  styleUrls: ['./home-wrapper.component.scss'],
})
export class HomePageWrapperComponent implements OnInit {
  featuredProducts: ListBuyerProduct;
  favoriteProductIDs: string[];
  quantityLimits: QuantityLimits[];

  constructor(private activatedRoute: ActivatedRoute, public context: ShopperContextService) {}

  ngOnInit() {
    this.featuredProducts = this.activatedRoute.snapshot.data.featuredProducts;
    this.quantityLimits = this.featuredProducts.Items.map((p) => BuildQtyLimits(p));
  }
}
