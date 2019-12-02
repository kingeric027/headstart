import { Component, ViewChild, ElementRef, Input, OnChanges, OnInit } from '@angular/core';
import {
  faSearch,
  faShoppingCart,
  faPhone,
  faQuestionCircle,
  faUserCircle,
  faSignOutAlt,
  faHome,
} from '@fortawesome/free-solid-svg-icons';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { Order, MeUser, LineItem, ListCategory, Category } from '@ordercloud/angular-sdk';
import { tap, debounceTime, delay, takeWhile } from 'rxjs/operators';
import { ProductFilters, ShopperContextService } from 'marketplace';

@Component({
  templateUrl: './app-header.component.html',
  styleUrls: ['./app-header.component.scss'],
})
export class OCMAppHeader implements OnInit {
  isCollapsed = true;
  anonymous: boolean;
  user: MeUser;
  order: Order;
  alive = true;
  addToCartQuantity: number;
  searchTermForProducts: string = null;
  activePath: string;
  appName: string;
  activeCategoryID: string = undefined;
  categories: Category[] = [];

  @ViewChild('addtocartPopover', { static: false }) public popover: NgbPopover;
  @ViewChild('cartIcon', { static: false }) cartIcon: ElementRef;

  faSearch = faSearch;
  faShoppingCart = faShoppingCart;
  faPhone = faPhone;
  faQuestionCircle = faQuestionCircle;
  faSignOutAlt = faSignOutAlt;
  faUserCircle = faUserCircle;
  faHome = faHome;

  constructor(private context: ShopperContextService) {}

  async ngOnInit() {
    this.categories = this.context.categories.all;
    this.appName = this.context.appSettings.appname;
    this.context.currentOrder.onOrderChange(order => (this.order = order));
    this.context.currentUser.onIsAnonymousChange(isAnon => (this.anonymous = isAnon));
    this.context.currentUser.onUserChange(user => (this.user = user));
    this.context.productFilters.activeFiltersSubject
      .pipe(takeWhile(() => this.alive))
      .subscribe(this.handleFiltersChange);
    this.context.router.onUrlChange(path => (this.activePath = path));
    this.buildAddToCartListener();
  }

  handleFiltersChange = (filters: ProductFilters) => {
    this.searchTermForProducts = filters.search || '';
    this.activeCategoryID = this.context.categories.activeID;
  }

  buildAddToCartListener() {
    let closePopoverTimeout;
    this.context.currentOrder.addToCartSubject.subscribe((li: LineItem) => {
      clearTimeout(closePopoverTimeout);
      if (li) {
        this.popover.ngbPopover = `Added ${li.Quantity} items to Cart`;
        setTimeout(() => {
          if (!this.popover.isOpen()) {
            this.popover.open();
          }
          closePopoverTimeout = setTimeout(() => {
            this.popover.close();
          }, 3000);
        }, 300);
      }
    });
  }

  searchProducts(searchStr: string) {
    this.searchTermForProducts = searchStr;
    this.context.router.toProductList({ search: searchStr });
  }

  logout() {
    this.context.authentication.logout();
  }

  closeMiniCart(event: MouseEvent, popover: NgbPopover) {
    const rect = this.cartIcon.nativeElement.getBoundingClientRect();
    // do not close if leaving through the bottom. That is handled by minicart itself
    if (event.y < rect.top + rect.height) {
      popover.close();
    }
  }

  setActiveCategory(categoryID: string): void {
    this.context.productFilters.filterByCategory(categoryID);
  }
}
