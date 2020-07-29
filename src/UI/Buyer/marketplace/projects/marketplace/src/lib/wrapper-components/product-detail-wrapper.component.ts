import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CurrentUserService } from '../services/current-user/current-user.service';
import { SuperMarketplaceProduct } from '@ordercloud/headstart-sdk';

@Component({
  template: `
    <ocm-product-details [product]="product"> </ocm-product-details>
  `,
})
export class ProductDetailWrapperComponent implements OnInit {
  product: SuperMarketplaceProduct;

  constructor(private activatedRoute: ActivatedRoute, protected currentUser: CurrentUserService) {}

  ngOnInit(): void {
    this.product = this.activatedRoute.snapshot.data.product;
  }
}
