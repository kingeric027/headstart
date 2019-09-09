import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { ListBuyerProduct, OcMeService, Category, ListCategory, ListFacet, ListLineItem } from '@ordercloud/angular-sdk';
import { ModalService, BuildQtyLimits, CurrentOrderService } from '@app-buyer/shared';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { isEmpty as _isEmpty, each as _each } from 'lodash';
import { QuantityLimits } from '@app-buyer/shared/models/quantity-limits';
import { ShopperContextService } from '@app-buyer/shared/services/shopper-context/shopper-context.service';
import { ProductFilters } from '@app-buyer/shared/services/product-filter/product-filter.service';

@Component({
  selector: 'product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss'],
})
export class ProductListComponent implements OnInit {
  products: ListBuyerProduct;
  categories: ListCategory;
  facets: ListFacet[];

  categoryCrumbs: Category[] = [];
  hasQueryParams = false;
  showingFavoritesOnly = false;
  closeIcon = faTimes;
  isModalOpen = false;
  createModalID = 'selectCategory';
  lineItems: ListLineItem;
  quantityLimits: QuantityLimits[];

  constructor(
    private activatedRoute: ActivatedRoute,
    private ocMeService: OcMeService,
    private router: Router,
    private modalService: ModalService,
    private currentOrder: CurrentOrderService,
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
    this.context.productFilterActions.onFiltersChange(this.handleFiltersChange);
  }

  private handleFiltersChange = async (filters: ProductFilters) => {
    this.hasQueryParams = true; // TODO - implement
    this.showingFavoritesOnly = filters.showOnlyFavorites;
    this.categoryCrumbs = this.buildBreadCrumbs(filters.categoryID);
    this.products = await this.ocMeService.ListProducts(this.context.productFilterActions.getOrderCloudParams()).toPromise();
    this.facets = this.products.Meta.Facets;
    this.quantityLimits = this.products.Items.map((p) => BuildQtyLimits(p));
  };

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
    this.context.productFilterActions.clearAllFilters();
  }

  changePage(page: number): void {
    this.context.productFilterActions.toPage(page);
  }

  toggleFilterByFavorites() {
    this.context.productFilterActions.filterByFavorites(!this.showingFavoritesOnly);
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
