import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { OcProductService, Meta } from '@ordercloud/angular-sdk';
import { ProductService } from '@app-seller/shared/services/product-filter/product-filter.service';
import { takeWhile } from 'rxjs/operators';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss'],
})
export class ProductListComponent implements OnInit, OnDestroy {
  alive = true;
  resources = [];

  constructor(private productService: ProductService, private changeDetectorRef: ChangeDetectorRef) {}

  ngOnInit() {
    this.subscribeToResources();
  }

  subscribeToResources() {
    this.productService.productSubject.pipe(takeWhile(() => this.alive)).subscribe((products) => {
      this.resources = products;
      console.log(this.resources);
    });
  }

  handleScrollEnd() {
    this.productService.getNextPage();
  }

  searchResources(event) {
    console.log(event);
  }

  ngOnDestroy() {
    this.alive = false;
  }
}
