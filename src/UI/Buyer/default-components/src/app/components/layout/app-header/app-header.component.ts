import { Component, ViewChild, ElementRef, OnInit } from '@angular/core';
import {
  faSearch,
  faShoppingCart,
  faPhone,
  faQuestionCircle,
  faUserCircle,
  faSignOutAlt,
  faHome,
  faBars,
} from '@fortawesome/free-solid-svg-icons';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { MeUser, LineItem, Category } from '@ordercloud/angular-sdk';
import { takeWhile } from 'rxjs/operators';
import { ProductFilters, ShopperContextService, MarketplaceOrder } from 'marketplace';
import { getScreenSizeBreakPoint } from 'src/app/services/breakpoint.helper';
import { CurrentUser } from 'marketplace/projects/marketplace/src/lib/services/current-user/current-user.service';

@Component({
  templateUrl: './app-header.component.html',
  styleUrls: ['./app-header.component.scss'],
})
export class OCMAppHeader implements OnInit {
  isCollapsed = true;
  isAnonymous: boolean;
  user: CurrentUser;
  order: MarketplaceOrder;
  alive = true;
  addToCartQuantity: number;
  searchTermForProducts: string = null;
  activePath: string;
  appName: string;
  activeCategoryID: string = undefined;
  categories: Category[] = [];
  screenSize = getScreenSizeBreakPoint();
  showCategoryDropdown = false;

  @ViewChild('addtocartPopover', { static: false }) public popover: NgbPopover;
  @ViewChild('cartIcon', { static: false }) cartIcon: ElementRef;

  faSearch = faSearch;
  faShoppingCart = faShoppingCart;
  faPhone = faPhone;
  faQuestionCircle = faQuestionCircle;
  faSignOutAlt = faSignOutAlt;
  faUserCircle = faUserCircle;
  faHome = faHome;
  faBars = faBars;

  constructor(public context: ShopperContextService) {}

  ngOnInit(): void {
    this.screenSize = getScreenSizeBreakPoint();
    this.categories = this.context.categories.all;
    this.appName = this.context.appSettings.appname;
    this.isAnonymous = this.context.currentUser.isAnonymous();
    this.context.order.onChange(order => (this.order = order));
    this.context.currentUser.onChange(user => (this.user = user));
    this.context.productFilters.activeFiltersSubject
      .pipe(takeWhile(() => this.alive))
      .subscribe(this.handleFiltersChange);
    this.context.router.onUrlChange(path => (this.activePath = path));
    this.buildAddToCartListener();
  }

  toggleCategoryDropdown(bool: boolean): void {
    this.showCategoryDropdown = bool;
  }

  clickOutsideCategoryDropdown(event): void {
    const clickIsOutside = !event.target.closest('.categoryDropdown');
    if (clickIsOutside) {
      this.showCategoryDropdown = false;
    }
  }

  handleFiltersChange = (filters: ProductFilters): void => {
    this.searchTermForProducts = filters.search || '';
    this.activeCategoryID = this.context.categories.activeID;
  };

  buildAddToCartListener(): void {
    let closePopoverTimeout;
    this.context.order.cart.onAdd.subscribe((li: LineItem) => {
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

  searchProducts(searchStr: string): void {
    this.searchTermForProducts = searchStr;
    this.context.router.toProductList({ search: searchStr });
  }

  logout(): void {
    this.context.authentication.logout();
  }

  closeMiniCart(event: MouseEvent, popover: NgbPopover): void {
    const rect = this.cartIcon.nativeElement.getBoundingClientRect();
    // do not close if leaving through the bottom. That is handled by minicart itself
    if (event.y < rect.top + rect.height) {
      popover.close();
    }
  }

  setActiveCategory(categoryID: string): void {
    this.context.productFilters.filterByCategory(categoryID);
  }

  toggleCollapsed(): void {
    this.isCollapsed = !this.isCollapsed;
  }
}
