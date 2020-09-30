import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CurrentUserService } from '../services/current-user/current-user.service';
import { SuperMarketplaceProduct, KitProduct } from '@ordercloud/headstart-sdk';
import { TempSdk } from '../services/temp-sdk/temp-sdk.service';

@Component({
  template: `
    <ocm-kit-product-details *ngIf="isKit" [product]="product"></ocm-kit-product-details>
    <ocm-product-details *ngIf="!isKit" [product]="product"> </ocm-product-details>
  `,
})
export class ProductDetailWrapperComponent implements OnInit {
  isKit: boolean;
  product: SuperMarketplaceProduct | KitProduct;
  constructor(private activatedRoute: ActivatedRoute, protected currentUser: CurrentUserService, private tempSdk: TempSdk) { }

  ngOnInit(): void {
    const product = this.activatedRoute.snapshot.data.product;
    this.isKit = product.xp.ProductTYpe === 'Kit';
    this.product = product;
  }
}
