import { Component, OnInit } from '@angular/core'
import { ActivatedRoute } from '@angular/router'
import { SuperHSProduct, HSKitProduct } from '@ordercloud/headstart-sdk'
import { CurrentUserService } from '../services/current-user/current-user.service'

@Component({
  template: `
    <ocm-kit-product-details
      *ngIf="isKit"
      [product]="product"
    ></ocm-kit-product-details>
    <ocm-product-details *ngIf="!isKit" [product]="product">
    </ocm-product-details>
  `,
})
export class ProductDetailWrapperComponent implements OnInit {
  isKit: boolean
  product: SuperHSProduct | HSKitProduct
  constructor(
    private activatedRoute: ActivatedRoute,
    protected currentUser: CurrentUserService
  ) {}

  ngOnInit(): void {
    const product = this.activatedRoute.snapshot.data.product
    this.isKit = product.Product.xp.ProductType === 'Kit'
    this.product = product
  }
}
