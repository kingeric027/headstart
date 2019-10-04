import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ListBuyerProduct } from '@ordercloud/angular-sdk';
import { BuildQtyLimits } from 'src/app/shared';
import { QuantityLimits } from 'src/app/shared/models/quantity-limits';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';

@Component({
  template: `
    <ocm-home-page [featuredProducts]="featuredProducts.Items" [quantityLimits]="quantityLimits" [context]="context"></ocm-home-page>
  `,
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
