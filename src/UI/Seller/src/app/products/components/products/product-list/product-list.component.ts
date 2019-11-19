import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { Product, ListProduct } from '@ordercloud/angular-sdk';
import { ProductService } from '@app-seller/shared/services/product/product.service';
import { takeWhile } from 'rxjs/operators';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss'],
})
export class ProductListComponent implements OnInit, OnDestroy {
  alive = true;
  productsList: ListProduct = { Meta: {}, Items: [] };
  searchText: string = null;

  // empty string if no resource is selected
  selectedResourceID = '';
  updatedResource = {};
  resourceInSelection = {};
  JSON = JSON;

  constructor(private productService: ProductService, private changeDetectorRef: ChangeDetectorRef) {}

  ngOnInit() {
    this.subscribeToResources();
    this.setFilters();
  }

  subscribeToResources() {
    this.productService.productSubject.pipe(takeWhile(() => this.alive)).subscribe((productsList) => {
      this.productsList = productsList;
      this.changeDetectorRef.detectChanges();
    });
  }

  handleScrollEnd() {
    this.productService.getNextPage();
  }

  setFilters() {
    this.searchText = this.productService.filterSubject.value.search;
  }

  searchResources(searchStr: string) {
    this.searchText = searchStr;
    this.productService.searchBy(searchStr);
  }

  selectResource(resource: any) {
    this.selectedResourceID = resource.ID;
    this.resourceInSelection = this.copyResource(resource);
    this.updatedResource = this.copyResource(resource);
  }

  updateResource(fieldName: string, event) {
    const newValue = event.target.value;
    this.updatedResource[fieldName] = newValue;
    console.log(this.updatedResource);
    console.log(this.resourceInSelection);
  }

  copyResource(resource: any) {
    return JSON.parse(JSON.stringify(resource));
  }

  saveUpdates() {
    this.productService.updateResource(this.updatedResource);
  }

  ngOnDestroy() {
    this.alive = false;
  }
}
