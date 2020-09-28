import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CurrentUserService } from '../services/current-user/current-user.service';
import { SuperMarketplaceProduct, MarketplaceKitProduct } from '@ordercloud/headstart-sdk';
import { TempSdk } from '../services/temp-sdk/temp-sdk.service';

@Component({
  template: `
    <ocm-product-details [product]="product"> </ocm-product-details>
  `,
})
export class ProductDetailWrapperComponent implements OnInit {
  product: any;
  constructor(private activatedRoute: ActivatedRoute, protected currentUser: CurrentUserService, private tempSdk: TempSdk) { }

  async ngOnInit(): Promise<void> {
    if (this.activatedRoute.snapshot.data.product.Product.xp.ProductType === 'Kit') {
      this.product = await this.tempSdk.getKitProduct(this.activatedRoute.snapshot.data.product.Product.ID);
    } else {
      this.product = this.activatedRoute.snapshot.data.product;
    }
  }
}
