import { Component, OnInit, Inject, ViewChild, OnDestroy, ElementRef } from '@angular/core';
import { applicationConfiguration, AppConfig } from '@app-buyer/config/app.config';
import { Router, ActivatedRoute } from '@angular/router';
import { faSearch, faShoppingCart, faPhone, faQuestionCircle, faUserCircle, faSignOutAlt, faHome } from '@fortawesome/free-solid-svg-icons';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { Observable } from 'rxjs';
import { Order, MeUser, ListCategory, LineItem } from '@ordercloud/angular-sdk';
import { takeWhile, tap, debounceTime, delay, filter } from 'rxjs/operators';
import { CurrentOrderService } from '@app-buyer/shared';
import { ShopperContextService } from '@app-buyer/shared/services/shopper-context/shopper-context.service';
import { AuthService } from '@app-buyer/shared/services/auth/auth.service';

@Component({
  selector: 'layout-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
})
export class HeaderComponent implements OnInit, OnDestroy {
  categories$: Observable<ListCategory>;
  isCollapsed = true;
  anonymous: boolean;
  user: MeUser;
  order: Order;
  alive = true;
  addToCartQuantity: number;
  searchTermForProducts: string = null;
  @ViewChild('addtocartPopover', { static: false }) public popover: NgbPopover;
  @ViewChild('cartIcon', { static: false }) cartIcon: ElementRef;

  faSearch = faSearch;
  faShoppingCart = faShoppingCart;
  faPhone = faPhone;
  faQuestionCircle = faQuestionCircle;
  faSignOutAlt = faSignOutAlt;
  faUserCircle = faUserCircle;
  faHome = faHome;

  constructor(
    private appAuthService: AuthService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private currentOrder: CurrentOrderService, // TODO- remove
    private context: ShopperContextService,
    @Inject(applicationConfiguration) public appConfig: AppConfig
  ) {}

  ngOnInit() {
    this.context.currentOrder.onOrderChange((order) => (this.order = order));
    this.context.currentUser.onIsAnonymousChange((isAnon) => (this.anonymous = isAnon));
    this.context.currentUser.onUserChange((user) => (this.user = user));
    this.context.productFilterActions.onFiltersChange((filters) => (this.searchTermForProducts = filters.search));

    this.buildAddToCartListener();
    this.clearSearchOnNavigate();
  }

  isMobile(): boolean {
    return window.innerWidth < 768; // max width for bootstrap's sm breakpoint
  }

  buildAddToCartListener() {
    this.currentOrder.addToCartSubject
      .pipe(
        tap((li: LineItem) => {
          this.popover.close();
          this.popover.ngbPopover = `Added! ${li.Quantity} in Cart`;
        }),
        delay(300),
        tap(() => {
          this.popover.open();
        }),
        debounceTime(3000)
      )
      .subscribe(() => {
        this.popover.close();
      });
  }

  searchProducts(searchStr: string) {
    this.searchTermForProducts = searchStr;
    this.context.routeActions.toProductList({ search: searchStr });
  }

  logout() {
    this.appAuthService.logout();
  }

  clearSearchOnNavigate() {
    this.activatedRoute.queryParams
      .pipe(
        filter((queryParams) => {
          return typeof queryParams.search === 'undefined';
        }),
        takeWhile(() => this.alive)
      )
      .subscribe(() => {
        this.searchTermForProducts = '';
      });
  }

  closeMiniCart(event: MouseEvent, popover: NgbPopover) {
    const rect = this.cartIcon.nativeElement.getBoundingClientRect();
    // do not close if leaving through the bottom
    if (event.y < rect.top + rect.height) {
      popover.close();
    }
  }

  // TODO: we should move responsibility for 'showing' up to the parent component instead of hard-coding route-names.
  showHeader() {
    const hiddenRoutes = ['/login', '/register', '/forgot-password', '/reset-password'];
    return !hiddenRoutes.some((el) => this.router.url.indexOf(el) > -1);
  }

  ngOnDestroy() {
    this.alive = false;
  }
}
