import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd, Params } from '@angular/router';
import {
  ListBuyerProduct,
  Category,
  ListCategory,
  ListLineItem,
} from '@ordercloud/angular-sdk';
import { CartService, AppStateService, ModalService } from '@app-buyer/shared';
import { AddToCartEvent } from '@app-buyer/shared/models/add-to-cart-event.interface';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { ProductSortStrategy } from '@app-buyer/product/models/product-sort-strategy.enum';
import { isEmpty as _isEmpty, each as _each } from 'lodash';
import { FavoriteProductsService } from '@app-buyer/shared/services/favorites/favorites.service';

@Component({
  selector: 'product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss'],
})
export class ProductListComponent implements OnInit, OnDestroy {
  products: ListBuyerProduct;
  categories: ListCategory;
  lineItems: ListLineItem;
  categoryCrumbs: Category[] = [];
  hasQueryParams = false;
  hasFavoriteProductsFilter = false;
  closeIcon = faTimes;
  isModalOpen = false;
  createModalID = 'selectCategory';
  navigationSubscription;

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private cartService: CartService,
    private appStateService: AppStateService,
    private modalService: ModalService,
    public favoriteProductsService: FavoriteProductsService
  ) {
    this.configureRouter();
  }

  ngOnInit() {
    this.products = this.activatedRoute.snapshot.data.products;
    this.categories = this.activatedRoute.snapshot.data.categories;
    this.categoryCrumbs = this.buildBreadCrumbs(
      this.activatedRoute.snapshot.queryParams.category
    );
    this.appStateService.lineItemSubject.subscribe(
      (lineItems) => (this.lineItems = lineItems)
    );
  }

  ngOnDestroy() {
    if (this.navigationSubscription) {
      this.navigationSubscription.unsubscribe();
    }
  }

  routeHome() {
    this.router.navigate(['/home']);
  }

  clearAllFilters() {
    this.router.navigate([]);
  }

  changePage(page: number): void {
    this.addQueryParam({ page });
  }

  changeCategory(category: string): void {
    this.addQueryParam({ category });
    if (this.isModalOpen) {
      this.closeCategoryModal();
    }
  }

  changeFacets(facetQueryParams: Params): void {
    this.addQueryParam(facetQueryParams);
  }

  changePrice(priceQueryParams: Params): void {
    this.addQueryParam(priceQueryParams);
  }

  changeSortStrategy(sortBy: ProductSortStrategy): void {
    this.addQueryParam({ sortBy });
  }

  changeFavorites() {
    this.activatedRoute.snapshot.queryParams.favoriteProducts
      ? this.addQueryParam({ favoriteProducts: undefined })
      : this.addQueryParam({ favoriteProducts: true });
  }

  private addQueryParam(newParam: object): void {
    const queryParams = {
      ...this.activatedRoute.snapshot.queryParams,
      ...newParam,
    };
    this.router.navigate([], { queryParams });
  }

  buildBreadCrumbs(catID: string): Category[] {
    const crumbs = [];

    if (!catID || !this.categories || this.categories.Items.length < 1) {
      return crumbs;
    }

    const recursiveBuild = (id) => {
      const cat = this.categories.Items.find((c) => c.ID === id);
      if (!cat) {
        return crumbs;
      }
      crumbs.unshift(cat);
      if (!cat.ParentID) {
        return crumbs;
      }

      return recursiveBuild(cat.ParentID);
    };

    return recursiveBuild(catID);
  }

  addToCart(event: AddToCartEvent) {
    this.cartService
      .addToCart(event.product.ID, event.quantity)
      .subscribe(() => this.appStateService.addToCartSubject.next(event));
  }

  openCategoryModal() {
    this.modalService.open('selectCategory');
    this.isModalOpen = true;
  }
  closeCategoryModal() {
    this.isModalOpen = false;
    this.modalService.close('selectCategory');
  }

  configureRouter() {
    /**
     *
     * override angular's default routing behavior so that
     * going to the same route with different query params are
     * detected as a state change. This fixes bug where >2 query
     * params of the same type aren't recognized
     *
     */
    this.router.events.subscribe((evt) => {
      if (evt instanceof NavigationEnd) {
        this.ngOnInit();
        this.router.navigated = false;
        window.scrollTo(0, 0);
      }
    });
  }
}
