import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd, Params } from '@angular/router';
import { ListBuyerProduct, OcMeService, Category, ListCategory, ListFacet, ListLineItem } from '@ordercloud/angular-sdk';
import { ModalService, BuildQtyLimits, CurrentOrderService } from '@app-buyer/shared';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { isEmpty as _isEmpty, each as _each } from 'lodash';
import { QuantityLimits } from '@app-buyer/shared/models/quantity-limits';
import { CurrentUserService } from '@app-buyer/shared/services/current-user/current-user.service';
import { ShopperContextService } from '@app-buyer/shared/services/shopper-context/shopper-context.service';
import { ProductListService } from '@app-buyer/shared/services/product-list/product-list.service';

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
  showingFavoritesOnly = false;
  closeIcon = faTimes;
  isModalOpen = false;
  createModalID = 'selectCategory';
  facets: ListFacet[];
  lineItems: ListLineItem;
  quantityLimits: QuantityLimits[];

  constructor(
    private activatedRoute: ActivatedRoute,
    private ocMeService: OcMeService,
    private router: Router,
    private modalService: ModalService,
    private currentOrder: CurrentOrderService,
    private currentUser: CurrentUserService,
    private productListService: ProductListService,
    protected context: ShopperContextService // used in template
  ) {}

  ngOnInit() {
    this.products = this.activatedRoute.snapshot.data.products;
    this.facets = this.products.Meta.Facets;
    this.categories = this.activatedRoute.snapshot.data.categories;

    this.quantityLimits = this.products.Items.map((p) => BuildQtyLimits(p));
    this.categoryCrumbs = this.buildBreadCrumbs(this.activatedRoute.snapshot.queryParams.categoryID);
    this.configureRouter();
    this.currentOrder.onLineItemsChange((lineItems) => (this.lineItems = lineItems));
    this.activatedRoute.queryParams.subscribe(this.onQueryParamsChange);
  }

  private onQueryParamsChange = async (queryParams: Params) => {
    this.hasQueryParams = !_isEmpty(queryParams);
    this.showingFavoritesOnly = queryParams.favorites === 'true';
    this.categoryCrumbs = this.buildBreadCrumbs(queryParams.categoryID);
    this.products = await this.ocMeService
      .ListProducts({
        categoryID: queryParams.categoryID,
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
    const favorites = this.currentUser.favoriteProductIDs;
    filter['ID'] = queryParams.favorites === 'true' && favorites ? favorites.join('|') : undefined;
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

  clearAllFilters() {
    this.router.navigate([]);
  }

  changePage(page: number): void {
    this.productListService.toPage(page);
  }

  changeFacets(facetQueryParams: Params): void {
    this.addQueryParam(facetQueryParams);
  }

  changePrice(priceQueryParams: Params): void {
    this.addQueryParam(priceQueryParams);
  }

  toggleFilterByFavorites() {
    this.productListService.filterByFavorites(!this.showingFavoritesOnly);
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

  openCategoryModal() {
    this.modalService.open('selectCategory');
    this.isModalOpen = true;
  }

  // TODO - it may be that this function is never used, but it should be.
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
