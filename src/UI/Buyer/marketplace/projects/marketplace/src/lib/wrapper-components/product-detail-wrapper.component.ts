import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CurrentUserService } from '../services/current-user/current-user.service';
import { SuperMarketplaceProduct, KitProduct } from '@ordercloud/headstart-sdk';
import { TempSdk } from '../services/temp-sdk/temp-sdk.service';

@Component({
  template: `
    <ocm-product-details [product]="product"> </ocm-product-details>
  `,
})
export class ProductDetailWrapperComponent implements OnInit {
  product: SuperMarketplaceProduct | KitProduct;
  constructor(private activatedRoute: ActivatedRoute, protected currentUser: CurrentUserService, private tempSdk: TempSdk) { }

  ngOnInit(): void {
    this.product = this.activatedRoute.snapshot.data.product;
  }
}
