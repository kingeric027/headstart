import { Component, ChangeDetectorRef, NgZone, OnInit, AfterViewChecked } from '@angular/core';
import { Product } from '@ordercloud/angular-sdk';
import { ProductService } from '@app-seller/shared/services/product/product.service';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Router, ActivatedRoute } from '@angular/router';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';

@Component({
  selector: 'app-product-table',
  templateUrl: './product-table.component.html',
  styleUrls: ['./product-table.component.scss'],
})
export class ProductTableComponent extends ResourceCrudComponent<Product> implements OnInit {
  userContext: {};
  constructor(
    private productService: ProductService,
    private currentUserService: CurrentUserService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedRoute: ActivatedRoute,
    ngZone: NgZone
  ) {
    super(changeDetectorRef, productService, router, activatedRoute, ngZone);
    this.getUserContext(currentUserService);
  }

  async getUserContext(currentUserService: CurrentUserService): Promise<void> {
    this.userContext = await currentUserService.getUserContext();
  }

  filterConfig = {
    id: 'SEB',
    timeStamp: '0001-01-01T00:00:00+00:00',
    MarketplaceName: 'Self Esteem Brands',
    Filters: [
      {
        Display: 'Status',
        Path: 'xp.Data.Status',
        Values: ['Draft', 'Published'],
      },
    ],
  };
}
