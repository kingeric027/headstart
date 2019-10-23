import { Component, AfterViewInit, Input } from '@angular/core';
import { ListOrder } from '@ordercloud/angular-sdk';
import { Observable } from 'rxjs';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { flatMap } from 'rxjs/operators';
import { ShopperContextService } from '../../../services/shopper-context/shopper-context.service';
import { OrderStatus } from '../../../shopper-context';

@Component({
  selector: 'order-history',
  templateUrl: './order-history.component.html',
  styleUrls: ['./order-history.component.scss'],
})
export class OrderHistoryComponent implements AfterViewInit {
  alive = true;
  columns: string[] = ['ID', 'Status', 'DateSubmitted', 'Total'];
  orders$: Observable<ListOrder>;
  hasFavoriteOrdersFilter = false;
  sortBy: string;
  @Input() approvalVersion: boolean;

  constructor(private router: Router, private activatedRoute: ActivatedRoute, public context: ShopperContextService) {}

  ngAfterViewInit(): void {
    if (!this.approvalVersion) {
      this.columns.push('Favorite');
    }
    this.orders$ = this.listOrders();
  }

  sortOrders(sortBy: string): void {
    this.addQueryParam({ sortBy });
  }

  changePage(page: number): void {
    this.addQueryParam({ page });
  }

  filterBySearch(search: string): void {
    this.addQueryParam({ search, page: undefined });
  }

  filterByStatus(status: OrderStatus): void {
    this.addQueryParam({ status });
  }

  filterByDate(datesubmitted: string[]): void {
    this.addQueryParam({ datesubmitted });
  }

  private addQueryParam(newParam: object): void {
    const queryParams = {
      ...this.activatedRoute.snapshot.queryParams,
      ...newParam,
    };
    this.router.navigate([], { queryParams });
  }

  filterByFavorite(favoriteOrders: boolean): void {
    // set to undefined so we dont pollute url with unnecessary query params
    this.addQueryParam({ favoriteOrders: favoriteOrders || undefined });
  }

  protected listOrders(): Observable<ListOrder> {
    return this.activatedRoute.queryParamMap.pipe(
      flatMap((queryParamMap) => {
        this.sortBy = queryParamMap.get('sortBy');
        // we set param values to undefined so the sdk ignores them (dont show in headers)
        const listOptions = {
          sortBy: this.sortBy || undefined,
          search: queryParamMap.get('search') || undefined,
          page: parseInt(queryParamMap.get('page'), 10) || undefined,
          filters: {
            ID: this.buildFavoriteOrdersQuery(queryParamMap),
            status: queryParamMap.get('status') || `!${OrderStatus.Unsubmitted}`,
            datesubmitted: queryParamMap.getAll('datesubmitted') || undefined,
          },
        };
        return this.approvalVersion
          ? this.context.myResources.ListApprovableOrders(listOptions)
          : this.context.myResources.ListOrders(listOptions);
      })
    );
  }

  private buildFavoriteOrdersQuery(queryParamMap: ParamMap): string | undefined {
    this.hasFavoriteOrdersFilter = queryParamMap.get('favoriteOrders') === 'true';
    if (!this.hasFavoriteOrdersFilter) return undefined;
    return this.context.currentUser.favoriteOrderIDs.join('|');
  }
}
