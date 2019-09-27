import { Component, ViewChild, ElementRef, Input, OnChanges } from '@angular/core';
import { faSearch, faShoppingCart, faPhone, faQuestionCircle, faUserCircle, faSignOutAlt, faHome } from '@fortawesome/free-solid-svg-icons';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { Observable } from 'rxjs';
import { Order, MeUser, ListCategory, LineItem } from '@ordercloud/angular-sdk';
import { tap, debounceTime, delay } from 'rxjs/operators';
import { OCMComponent } from '../base-component';
import { ProductFilters } from '../../shopper-context';

@Component({
  selector: 'ocm-app-header',
  templateUrl: './app-header.component.html',
  styleUrls: ['./app-header.component.scss'],
})
export class OCMAppHeader extends OCMComponent implements OnChanges {
  categories$: Observable<ListCategory>;
  isCollapsed = true;
  anonymous: boolean;
  user: MeUser;
  order: Order;
  alive = true;
  addToCartQuantity: number;
  searchTermForProducts: string = null;
  activePath: string;
  appName: string;
  @Input() showHeader: boolean; // TODO - find a way to remove this
  @ViewChild('addtocartPopover', { static: false }) public popover: NgbPopover;
  @ViewChild('cartIcon', { static: false }) cartIcon: ElementRef;

  faSearch = faSearch;
  faShoppingCart = faShoppingCart;
  faPhone = faPhone;
  faQuestionCircle = faQuestionCircle;
  faSignOutAlt = faSignOutAlt;
  faUserCircle = faUserCircle;
  faHome = faHome;

  ngOnChanges() {
    this.appName = this.context.appSettings.appname;
    this.context.currentOrder.onOrderChange((order) => (this.order = order));
    this.context.currentUser.onIsAnonymousChange((isAnon) => (this.anonymous = isAnon));
    this.context.currentUser.onUserChange((user) => (this.user = user));
    this.context.productFilterActions.onFiltersChange(this.handleFiltersChange);
    this.context.routeActions.onUrlChange((path) => (this.activePath = path));
    this.buildAddToCartListener();
  }

  handleFiltersChange = (filters: ProductFilters) => {
    this.searchTermForProducts = filters.search || '';
  };

  buildAddToCartListener() {
    this.context.cartActions.addToCartSubject
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
    this.context.authentication.logout();
  }

  closeMiniCart(event: MouseEvent, popover: NgbPopover) {
    const rect = this.cartIcon.nativeElement.getBoundingClientRect();
    // do not close if leaving through the bottom. That is handled by minicart itself
    if (event.y < rect.top + rect.height) {
      popover.close();
    }
  }
}
