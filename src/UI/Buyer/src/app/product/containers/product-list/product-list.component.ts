import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd, Params } from '@angular/router';
import { ListBuyerProduct, OcMeService, Category, ListCategory, ListFacet, ListLineItem, LineItem } from '@ordercloud/angular-sdk';
import { CartService, AppStateService, ModalService, BuildQtyLimits } from '@app-buyer/shared';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { FavoriteProductsService } from '@app-buyer/shared/services/favorites/favorites.service';
import { ProductSortStrategy } from '@app-buyer/product/models/product-sort-strategy.enum';
import { isEmpty as _isEmpty, each as _each } from 'lodash';
import { QuantityLimits } from '@app-buyer/shared/models/quantity-limits';

@Component({
  selector: 'product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss'],
})
export class ProductListComponent implements OnInit {
  products: ListBuyerProduct;
  categories: ListCategory;
  categoryCrumbs: Category[] = [];
  hasQueryParams = false;
  hasFavoriteProductsFilter = false;
  closeIcon = faTimes;
  isModalOpen = false;
  createModalID = 'selectCategory';
  facets: ListFacet[];
  lineItems: ListLineItem;
  quantityLimits: QuantityLimits[];
  sortBy: any;

  constructor(
    private activatedRoute: ActivatedRoute,
    private ocMeService: OcMeService,
    private router: Router,
    private cartService: CartService,
    public favoriteProductsService: FavoriteProductsService,
    private appStateService: AppStateService,
    private modalService: ModalService
  ) {}

  ngOnInit() {
    this.products = this.activatedRoute.snapshot.data.products;
    this.facets = this.products.Meta.Facets;
    this.categories = this.activatedRoute.snapshot.data.categories;
    this.sortBy = this.activatedRoute.queryParams['sortBy'];

    this.quantityLimits = this.products.Items.map((p) => BuildQtyLimits(p));
    this.categoryCrumbs = this.buildBreadCrumbs(this.activatedRoute.snapshot.queryParams.category);
    this.configureRouter();
    this.appStateService.lineItemSubject.subscribe((lineItems) => (this.lineItems = lineItems));
    this.activatedRoute.queryParams.subscribe(this.onQueryParamsChange);
  }

  private onQueryParamsChange = async (queryParams: Params) => {
    this.hasQueryParams = !_isEmpty(queryParams);
    this.hasFavoriteProductsFilter = queryParams.favoriteProducts === 'true';
    this.categoryCrumbs = this.buildBreadCrumbs(queryParams.category);
    this.products = await this.ocMeService
      .ListProducts({
        categoryID: queryParams.category,
        page: queryParams.page,
        search: queryParams.search,
        sortBy: queryParams.sortBy,
        filters: {
          ...this.buildFacetFilters(queryParams),
          ...this.buildFavoritesFilter(queryParams),
          ...this.buildPriceFilter(queryParams),
        },
      })
      .toPromise();
    this.facets = this.products.Meta.Facets;
    this.quantityLimits = this.products.Items.map((p) => BuildQtyLimits(p));
  };

  private buildFacetFilters(queryParams: Params): Params {
    if (!this.facets) {
      // either premium search is not enabled for this client
      // or ProductFacets have not been defined
      return {};
    }
    const result = {};
    _each(queryParams, (queryParamVal, queryParamName) => {
      const facetDefinition = this.facets.find((facet) => facet.Name === queryParamName);
      if (facetDefinition) {
        result[`xp.${facetDefinition.XpPath}`] = queryParamVal;
      }
    });
    return result;
  }

  private buildFavoritesFilter(queryParams: Params): Params {
    const filter = {};
    const favorites = this.favoriteProductsService.getFavorites();
    filter['ID'] = queryParams.favoriteProducts === 'true' && favorites ? favorites.join('|') : undefined;
    return filter;
  }

  private buildPriceFilter(queryParams: Params): Params {
    const filter = {};
    if (queryParams.minPrice && !queryParams.maxPrice) {
      filter['xp.Price'] = `>=${queryParams.minPrice}`;
    }
    if (queryParams.maxPrice && !queryParams.minPrice) {
      filter['xp.Price'] = `<=${queryParams.maxPrice}`;
    }
    if (queryParams.minPrice && queryParams.maxPrice) {
      filter['xp.Price'] = [`>=${queryParams.minPrice}`, `<=${queryParams.maxPrice}`];
    }
    return filter;
  }

  getCategories(): void {
    this.ocMeService.ListCategories({ depth: 'all' }).subscribe((categories) => {
      this.categories = categories;
      const categoryID = this.activatedRoute.snapshot.queryParams.category;
      this.categoryCrumbs = this.buildBreadCrumbs(categoryID);
    });
  }

  routeHome() {
    this.router.navigate(['/home']);
  }

  toDetails(productID: string) {
    if (!productID) return;
    this.router.navigate([`/products/${productID}`]);
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

  refineByFavorites() {
    const queryParams = this.activatedRoute.snapshot.queryParams;
    if (queryParams.favoriteProducts === 'true') {
      // favorite products was previously set to true so toggle off
      // set to undefined so we dont pollute url with unnecessary query params
      this.addQueryParam({ favoriteProducts: undefined });
    } else {
      this.addQueryParam({ favoriteProducts: true });
    }
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

  addToCart(li: LineItem) {
    this.cartService.addToCart(li);
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
        this.router.navigated = false;
        window.scrollTo(0, 0);
      }
    });
  }
}
