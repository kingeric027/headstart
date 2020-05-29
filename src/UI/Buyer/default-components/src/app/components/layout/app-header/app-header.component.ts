import { Component, ViewChild, ElementRef, OnInit } from '@angular/core';
import {
  faSearch,
  faShoppingCart,
  faPhone,
  faQuestionCircle,
  faUserCircle,
  faSignOutAlt,
  faBoxOpen,
  faHome,
  faBars,
} from '@fortawesome/free-solid-svg-icons';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { Category } from '@ordercloud/angular-sdk';
import { takeWhile } from 'rxjs/operators';
import { ProductFilters, ShopperContextService, AppConfig } from 'marketplace';
import { MarketplaceOrder, MarketplaceLineItem} from 'marketplace-javascript-sdk'
import { getScreenSizeBreakPoint } from 'src/app/services/breakpoint.helper';
import { CurrentUser } from 'marketplace/projects/marketplace/src/lib/services/current-user/current-user.service';
import { RouteConfig } from 'marketplace/projects/marketplace/src/lib/services/route/route-config';
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
  profileRoutes: RouteConfig[] = [];
  orderRoutes: RouteConfig[] = [];
  numberOfOrdersToApprove = 0;

  @ViewChild('addToCartPopover', { static: false }) public addToCartPopover: NgbPopover;
  @ViewChild('ordersToApprovePopover', { static: false }) public ordersToApprovePopover: NgbPopover;
  @ViewChild('cartIcon', { static: false }) cartIcon: ElementRef;

  faSearch = faSearch;
  faShoppingCart = faShoppingCart;
  faPhone = faPhone;
  faQuestionCircle = faQuestionCircle;
  faSignOutAlt = faSignOutAlt;
  faUserCircle = faUserCircle;
  faHome = faHome;
  faBars = faBars;
  faBoxOpen = faBoxOpen;
  flagIcon: string;

  constructor(public context: ShopperContextService, private appConfig: AppConfig) {
    this.profileRoutes = context.router.getProfileRoutes();
    this.orderRoutes = context.router.getOrderRoutes();
  }

  async ngOnInit(): Promise<void> {
    this.buildShowOrdersNeedingApprovalAlertListener();
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
    this.flagIcon = await this.getCurrencyFlag();
  }

  async getCurrencyFlag(): Promise<string> {
    const rates = this.context.exchangeRates.Get();
    const currentUser = this.context.currentUser.get();
    // Using `|| "USD"` for fallback right now in case there's bad data without the xp value.
    const myCurrencyCode = currentUser.UserGroups.filter(ug => ug.xp?.Type === 'BuyerLocation')[0].xp?.Currency || 'USD';
    const myRate = rates.Items.find(r => r.Currency === myCurrencyCode);
    return myRate.Icon;
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
    this.context.order.cart.onAdd.subscribe((li: MarketplaceLineItem) => {
      clearTimeout(closePopoverTimeout);
      if (li) {
        this.addToCartPopover.ngbPopover = `Added ${li.Quantity} items to Cart`;
        setTimeout(() => {
          if (!this.addToCartPopover.isOpen()) {
            this.addToCartPopover.open();
          }
          closePopoverTimeout = setTimeout(() => {
            this.addToCartPopover.close();
          }, 3000);
        }, 300);
      }
    });
  }

  routeToOrdersToApprove(event): void {
    this.context.router.toOrdersToApprove();
    event.stopPropagation();
  }

  buildShowOrdersNeedingApprovalAlertListener(): void {
    this.buildShowOrdersNeedingApprovalListenerForAlert();
    this.buildShowOrdersNeedingApprovalListenerForIndicator();
  }

  buildShowOrdersNeedingApprovalListenerForIndicator(): void {
    this.context.ordersToApprove.numberOfOrdersToApprove.subscribe(num => {
      this.numberOfOrdersToApprove = num;
    });
  }

  buildShowOrdersNeedingApprovalListenerForAlert(): void {
    /* the orders to approve behaviour subject will only receive a truthy
     * value when OrdersToApproveStateService.alertIfOrdersToApprove is called.
     * This currently only happens on login. In order to prevent the alert from
     * showing again we set the value back to zero when this is called
     */
    this.context.ordersToApprove.showAlert.subscribe((num: number) => {
      // brief timeout to allow the popover to be defined in the template
      // additionally i think this slight timeout is a better ux
      setTimeout(() => {
        if (num) {
          this.numberOfOrdersToApprove = num;
          this.context.ordersToApprove.showAlert.next(0);
          this.ordersToApprovePopover.open();
          setTimeout(() => {
            this.ordersToApprovePopover.close();
          }, 10000);
        }
      }, 1500);
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
